using do_an_tot_nghiep.Filters;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    /// <summary>
    /// MVC controller cho màn hình thanh lý hợp đồng.
    /// </summary>
    [RequireRole(Roles.AdminOrStaff)]
    public class LiquidationsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public LiquidationsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET /Liquidations/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var resp = await _httpClient.GetAsync("api/liquidations");
                var items = new List<object>();
                if (resp.IsSuccessStatusCode)
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    using var doc = JsonDocument.Parse(body);
                    // API trả về array — map thành ViewBag list
                    foreach (var el in doc.RootElement.EnumerateArray())
                    {
                        items.Add(new
                        {
                            ContractCode = el.TryGetProperty("contractCode", out var cc) ? cc.GetString() : "N/A",
                            RoomName = el.TryGetProperty("roomName", out var rn) ? rn.GetString() : "N/A",
                            LiquidationDate = el.TryGetProperty("liquidationDate", out var dt) ? dt.GetString() : "",
                            Amount = el.TryGetProperty("refundAmount", out var ra) ? ra.GetDecimal().ToString("N0") : "0"
                        });
                    }
                }
                ViewBag.Liquidations = items;
                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                ViewBag.Liquidations = new List<object>();
                return View();
            }
        }

        // ─────────────────────────────────────────────────────
        // GET /Liquidations/Create?contractId=xxx
        // Trang xem trước quyết toán thanh lý
        // ─────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Create(int contractId)
        {
            if (contractId <= 0)
            {
                TempData["Error"] = "ContractId không hợp lệ.";
                return RedirectToAction("Index", "HopDong");
            }

            try
            {
                var resp = await _httpClient.GetAsync($"api/liquidations/preview/{contractId}");
                var body = await resp.Content.ReadAsStringAsync();

                if (!resp.IsSuccessStatusCode)
                {
                    var err = TryParseMessage(body);
                    TempData["Error"] = $"Không thể tải preview thanh lý: {err}";
                    return RedirectToAction("Index", "HopDong");
                }

                var data = JsonSerializer.Deserialize<ViewModels.LiquidationPreviewViewModel>(body, _jsonOptions);
                if (data == null)
                {
                    TempData["Error"] = "Dữ liệu preview không đọc được.";
                    return RedirectToAction("Index", "HopDong");
                }

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return RedirectToAction("Index", "HopDong");
            }
        }

        // ─────────────────────────────────────────────────────
        // POST /Liquidations/ConfirmLiquidation
        // Gửi lệnh thực hiện thanh lý
        // ─────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmLiquidation(ViewModels.LiquidationConfirmViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Dữ liệu form không hợp lệ.";
                return RedirectToAction("Create", new { contractId = model.ContractId });
            }

            // FE validation (phòng thủ thứ hai, backend cũng check)
            if (model.FinalElectricityIndex < model.LastElectricityIndex)
            {
                TempData["Error"] = "Chỉ số điện bàn giao phải >= chỉ số lần trước.";
                return RedirectToAction("Create", new { contractId = model.ContractId });
            }
            if (model.FinalWaterIndex < model.LastWaterIndex)
            {
                TempData["Error"] = "Chỉ số nước bàn giao phải >= chỉ số lần trước.";
                return RedirectToAction("Create", new { contractId = model.ContractId });
            }

            // Lấy StaffId từ session/claim (tạm thời từ User.Claims nếu có)
            int staffId = 1;
            var staffClaim = User.Claims.FirstOrDefault(c => c.Type == "UserId");
            if (staffClaim != null && int.TryParse(staffClaim.Value, out int sid)) staffId = sid;

            try
            {
                var payload = new
                {
                    ContractId = model.ContractId,
                    LastElectricityIndex = model.LastElectricityIndex,
                    FinalElectricityIndex = model.FinalElectricityIndex,
                    LastWaterIndex = model.LastWaterIndex,
                    FinalWaterIndex = model.FinalWaterIndex,
                    PenaltyAmount = model.PenaltyAmount,
                    Reason = model.Reason,
                    StaffId = staffId
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(payload),
                    Encoding.UTF8,
                    "application/json");

                var resp = await _httpClient.PostAsync("api/liquidations/execute", content);
                var body = await resp.Content.ReadAsStringAsync();

                if (resp.IsSuccessStatusCode)
                {
                    TempData["Success"] = "✅ Thanh lý hợp đồng thành công! Phòng đã được giải phóng về trạng thái Trống.";
                    return RedirectToAction("Index", "HopDong");
                }

                var msg = TryParseMessage(body);
                TempData["Error"] = $"Lỗi thanh lý: {msg}";
                return RedirectToAction("Create", new { contractId = model.ContractId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
                return RedirectToAction("Create", new { contractId = model.ContractId });
            }
        }

        // helper
        private static string TryParseMessage(string body)
        {
            try
            {
                var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var msg)) return msg.GetString() ?? body;
            }
            catch { }
            return body.Length > 200 ? body[..200] + "..." : body;
        }
    }
}
