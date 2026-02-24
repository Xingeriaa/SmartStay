using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using do_an_tot_nghiep.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class HopDongController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public HopDongController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: HopDong
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/hopdong");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Lỗi kết nối Backend API.";
                    return View(new List<HopDongListItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                // We map from HopDongDTO
                var data = JsonSerializer.Deserialize<List<HopDongListItemViewModel>>(json, _jsonOptions);
                return View(data ?? new List<HopDongListItemViewModel>());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống lập báo cáo: {ex.Message}";
                return View(new List<HopDongListItemViewModel>());
            }
        }

        private async Task BuildSelectLists(HopDongFormViewModel model)
        {
            // Populate Rooms (only Vacant rooms should ideally be selectable for New Contract)
            var rmResp = await _httpClient.GetAsync("api/phongtro");
            if (rmResp.IsSuccessStatusCode)
            {
                var rmJson = await rmResp.Content.ReadAsStringAsync();
                var rooms = JsonSerializer.Deserialize<List<Models.PhongTro>>(rmJson, _jsonOptions) ?? new List<Models.PhongTro>();

                model.AvailableRooms = rooms
                    .Where(r => r.Status == "Vacant" && !r.IsDeleted) // Chỉ lấy phòng trống
                    .Select(r => new SelectListItem
                    {
                        Value = r.Id.ToString(),
                        Text = $"Phòng {r.TenPhong} - Tầng {r.Tang} - Giá {r.GiaPhong.ToString("N0")}đ"
                    });
            }

            // Populate Tenants
            var tnResp = await _httpClient.GetAsync("api/khachthue");
            if (tnResp.IsSuccessStatusCode)
            {
                var tnJson = await tnResp.Content.ReadAsStringAsync();
                var tenants = JsonSerializer.Deserialize<List<Models.KhachThue>>(tnJson, _jsonOptions) ?? new List<Models.KhachThue>();

                model.AvailableTenants = tenants
                    .Where(t => !t.IsDeleted)
                    .Select(t => new SelectListItem
                    {
                        Value = t.Id.ToString(),
                        Text = $"{t.HoTen} (CCCD: {t.SoCCCD} - SĐT: {t.SoDienThoai1})"
                    });
            }
        }

        // GET: HopDong/Create
        public async Task<IActionResult> Create()
        {
            var model = new HopDongFormViewModel();
            await BuildSelectLists(model);
            return View(model);
        }

        // POST: HopDong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HopDongFormViewModel model)
        {
            if (!ModelState.IsValid || model.KhachThueIds == null || model.KhachThueIds.Count == 0)
            {
                ModelState.AddModelError("KhachThueIds", "Phải chọn ít nhất 1 cư dân cho hợp đồng.");
                await BuildSelectLists(model);
                return View(model);
            }

            try
            {
                // DTO to match CreateContractRequest in API
                var payload = new
                {
                    PhongTroId = model.PhongTroId,
                    NgayBatDau = model.NgayBatDau,
                    NgayKetThuc = model.NgayKetThuc,
                    PaymentCycle = model.PaymentCycle,
                    TienCoc = model.TienCoc,
                    KhachThueIds = model.KhachThueIds
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/hopdong", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Ký Hợp đồng mới thành công. Phòng đã chuyển trạng thái Occupied và chốt giá Snapshot Dịch vụ (Rule 2.2 + 2.3).";
                    return RedirectToAction(nameof(Index));
                }

                var errMsg = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"BMS Backend từ chối giao dịch: {errMsg}";
                await BuildSelectLists(model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Exception: {ex.Message}";
                await BuildSelectLists(model);
                return View(model);
            }
        }

        // GET: HopDong/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/hopdong/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Không tìm thấy hợp đồng ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<HopDongDetailDto>(json, _jsonOptions);
                if (contract == null)
                {
                    TempData["Error"] = "Dữ liệu hợp đồng không hợp lệ.";
                    return RedirectToAction(nameof(Index));
                }

                // Load invoices for this contract
                var invResp = await _httpClient.GetAsync($"api/invoices?contractId={id}");
                List<InvoiceItemDto> invoices = new();
                if (invResp.IsSuccessStatusCode)
                {
                    var invJson = await invResp.Content.ReadAsStringAsync();
                    invoices = JsonSerializer.Deserialize<List<InvoiceItemDto>>(invJson, _jsonOptions) ?? new();
                }

                ViewBag.Invoices = invoices;
                return View(contract);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: HopDong/Terminate/5 → redirect sang Liquidations/Create (quy trình thanh lý chuẩn)
        public async Task<IActionResult> Terminate(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/hopdong/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Không tìm thấy hợp đồng ID {id}.";
                    return RedirectToAction(nameof(Index));
                }

                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<HopDongDetailDto>(json, _jsonOptions);

                if (contract == null)
                {
                    TempData["Error"] = "Dữ liệu hợp đồng không đọc được.";
                    return RedirectToAction(nameof(Index));
                }

                if (contract.TrangThai != "Active")
                {
                    TempData["Error"] = $"Hợp đồng {contract.ContractCode} không ở trạng thái Đang hiệu lực. Không thể thanh lý.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Delegate sang Liquidations/Create — đây là quy trình chính thức
                return RedirectToAction("Create", "Liquidations", new { contractId = id });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: HopDong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/hopdong/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Hủy bỏ Hợp Đồng thành công. Đã thực hiện Soft Delete và trả lại phòng Vacant.";
                }
                else
                {
                    TempData["Error"] = "Cấm thực thi vòng luân chuyển: Lỗi khi Hủy hợp đồng.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Mất kết nối: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: HopDong/Extend/5
        public async Task<IActionResult> Extend(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/hopdong/{id}");
                if (!response.IsSuccessStatusCode) return NotFound("Contract data not found");

                var json = await response.Content.ReadAsStringAsync();
                var contract = JsonSerializer.Deserialize<HopDongDetailDto>(json, _jsonOptions);

                if (contract == null) return NotFound();

                var vm = new ContractExtensionViewModel
                {
                    ContractId = contract.Id,
                    ContractCode = contract.ContractCode,
                    OldEndDate = contract.NgayKetThuc ?? contract.NgayBatDau,
                    OldRentPrice = contract.GiaThue,
                    NewEndDate = contract.NgayKetThuc ?? contract.NgayBatDau.AddMonths(contract.PaymentCycle),
                    NewRentPrice = contract.GiaThue
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi tải dự liệu gia hạn: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: HopDong/Extend/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Extend(int id, ContractExtensionViewModel model)
        {
            if (id != model.ContractId) return BadRequest();

            if (model.NewEndDate <= model.OldEndDate)
            {
                ModelState.AddModelError("NewEndDate", "Ngày mới phải thực sự nằm sau ngày cũ tương lai.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var payload = new
                {
                    NewEndDate = model.NewEndDate,
                    NewRentPrice = model.NewRentPrice
                };
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/hopdong/{id}/extend", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = $"Đã ban hành Phụ Lục Gia Hạn thành công mã Hợp đồng {model.ContractCode}";
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Từ chối Phụ Lục: {error}";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi xử lý kết nối: " + ex.Message);
                return View(model);
            }
        }
    }

    /// <summary>Flat DTO matching the JSON shape returned by HopDongApiController.GetById / GetAll.</summary>
    internal class HopDongDetailDto
    {
        public int Id { get; set; }
        public string ContractCode { get; set; } = string.Empty;
        public int PhongTroId { get; set; }
        public string? TenPhong { get; set; }
        public DateTime NgayBatDau { get; set; }
        public DateTime? NgayKetThuc { get; set; }
        public DateTime SignatureDate { get; set; }
        public int PaymentCycle { get; set; }
        public decimal GiaThue { get; set; }
        public decimal TienCoc { get; set; }
        public string? DepositStatus { get; set; }
        public string TrangThai { get; set; } = string.Empty;
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    internal class InvoiceItemDto
    {
        public int Id { get; set; }
        public string? InvoiceCode { get; set; }
        public string? MonthYear { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
