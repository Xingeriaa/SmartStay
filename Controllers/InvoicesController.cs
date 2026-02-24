using do_an_tot_nghiep.Filters;
using do_an_tot_nghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class InvoicesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public InvoicesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index(int? contractId, string? status, string? monthYear)
        {
            try
            {
                var q = new List<string>();
                if (contractId.HasValue && contractId > 0) q.Add($"contractId={contractId}");
                if (!string.IsNullOrWhiteSpace(status)) q.Add($"status={status}");
                if (!string.IsNullOrWhiteSpace(monthYear)) q.Add($"monthYear={monthYear}");
                string queryStr = q.Count > 0 ? "?" + string.Join("&", q) : "";

                var resp = await _httpClient.GetAsync($"api/invoices{queryStr}");
                var data = new List<InvoiceListItemViewModel>();

                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    data = JsonSerializer.Deserialize<List<InvoiceListItemViewModel>>(json, _jsonOptions) ?? new List<InvoiceListItemViewModel>();

                    // Fetch HopDongs to get Room Name mapped locally (for quick UI mapping)
                    var cResp = await _httpClient.GetAsync("api/hopdong");
                    var rResp = await _httpClient.GetAsync("api/phongtro");

                    if (cResp.IsSuccessStatusCode && rResp.IsSuccessStatusCode)
                    {
                        var contJson = await cResp.Content.ReadAsStringAsync();
                        var roomJson = await rResp.Content.ReadAsStringAsync();

                        var contracts = JsonSerializer.Deserialize<List<do_an_tot_nghiep.ViewModels.HopDongListItemViewModel>>(contJson, _jsonOptions);
                        var rooms = JsonSerializer.Deserialize<List<Models.PhongTro>>(roomJson, _jsonOptions);

                        if (contracts != null && rooms != null)
                        {
                            foreach (var item in data)
                            {
                                var ct = contracts.FirstOrDefault(c => c.Id == item.ContractId);
                                if (ct != null)
                                {
                                    var room = rooms.FirstOrDefault(r => r.Id == ct.PhongTroId);
                                    if (room != null) item.RoomName = $"Phòng {room.TenPhong}";
                                }
                            }
                        }
                    }
                }

                ViewBag.CurrentContractId = contractId;
                ViewBag.CurrentStatus = status;
                ViewBag.CurrentMonth = monthYear;
                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return View(new List<InvoiceListItemViewModel>());
            }
        }

        private async Task PopSelectLists(GenerateInvoiceViewModel model)
        {
            var res = await _httpClient.GetAsync("api/hopdong");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var list = JsonSerializer.Deserialize<List<do_an_tot_nghiep.ViewModels.HopDongListItemViewModel>>(json, _jsonOptions);
                if (list != null)
                {
                    model.AvailableContracts = list
                        .Where(h => h.TrangThai != "Cancelled" && h.TrangThai != "Draft") // Chỉ lấy hợp đồng đang hiệu lực hoặc đã thanh lý
                        .Select(h => new SelectListItem
                        {
                            Value = h.Id.ToString(),
                            Text = $"HĐ: {h.ContractCode} - {(h.TrangThai == "Active" ? "Đang hiệu lực" : h.TrangThai)}"
                        });
                }
            }
        }

        public async Task<IActionResult> Generate()
        {
            var model = new GenerateInvoiceViewModel();
            await PopSelectLists(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Generate(GenerateInvoiceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopSelectLists(model);
                return View(model);
            }

            try
            {
                var payload = new
                {
                    ContractId = model.ContractId,
                    MonthYear = model.MonthYear,
                    DueDate = model.DueDate
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/invoices/generate", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"Sinh Hóa Đơn {model.MonthYear} thành công và tự động cộng trừ cân bằng Công Nợ (Ledger Rule 4).";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = $"BMS Generator: {await response.Content.ReadAsStringAsync()}";
                await PopSelectLists(model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Crash: " + ex.Message;
                await PopSelectLists(model);
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var resp = await _httpClient.GetAsync($"api/invoices/{id}");
                if (!resp.IsSuccessStatusCode) return NotFound();

                var json = await resp.Content.ReadAsStringAsync();
                var invRaw = JsonSerializer.Deserialize<Models.Invoice>(json, _jsonOptions);
                if (invRaw == null) return NotFound();

                var vm = new InvoiceDetailViewModel
                {
                    Id = invRaw.Id,
                    InvoiceCode = invRaw.InvoiceCode,
                    MonthYear = invRaw.MonthYear ?? "",
                    TotalAmount = invRaw.TotalAmount,
                    Status = invRaw.Status ?? "",
                    DueDate = invRaw.DueDate ?? DateTime.Today,
                    SubTotal = invRaw.SubTotal,
                    TaxAmount = invRaw.TaxAmount,
                    Lines = invRaw.Details?.Select(d => new InvoiceLineItem
                    {
                        Description = d.Description ?? "",
                        Quantity = d.Quantity,
                        UnitPrice = d.UnitPriceSnapshot,
                        Amount = d.Amount
                    }).ToList() ?? new List<InvoiceLineItem>()
                };

                // Lấy thông tin phòng qua Contract
                var cResp = await _httpClient.GetAsync($"api/hopdong/{invRaw.ContractId}");
                if (cResp.IsSuccessStatusCode)
                {
                    var cJson = await cResp.Content.ReadAsStringAsync();
                    var ct = JsonSerializer.Deserialize<do_an_tot_nghiep.ViewModels.HopDongListItemViewModel>(cJson, _jsonOptions);
                    if (ct != null)
                    {
                        vm.RoomName = $"Phòng {ct.TenPhong ?? "N/A"}";
                    }
                }

                return View(vm);
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkPaid(int id)
        {
            try
            {
                // Gọi API lấy detail để biết TotalAmount
                var resp = await _httpClient.GetAsync($"api/invoices/{id}");
                if (!resp.IsSuccessStatusCode) return NotFound();

                var json = await resp.Content.ReadAsStringAsync();
                var invRaw = JsonSerializer.Deserialize<Models.Invoice>(json, _jsonOptions);
                if (invRaw == null) return NotFound();

                var payload = new
                {
                    InvoiceId = id,
                    Amount = invRaw.TotalAmount,
                    PaymentMethod = "BankTransfer_Manual",
                    TransactionCode = "MANUAL"
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var res = await _httpClient.PostAsync("api/payments/manual-pay", content);

                if (res.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đã đối soát gạch nợ thành công, xác nhận tiền vào quỹ.";
                }
                else
                {
                    var msg = await res.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Gạch nợ thất bại: {msg}";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Details), new { id = id });
        }
    }
}
