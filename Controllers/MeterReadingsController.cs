using do_an_tot_nghiep.Filters;
using do_an_tot_nghiep.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text;
using System.Text.Json;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class MeterReadingsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public MeterReadingsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index(int? roomId, string? monthYear)
        {
            try
            {
                // Build query
                var q = new List<string>();
                if (roomId.HasValue && roomId > 0) q.Add($"roomId={roomId}");
                if (!string.IsNullOrWhiteSpace(monthYear)) q.Add($"monthYear={monthYear}");
                string queryStr = q.Count > 0 ? "?" + string.Join("&", q) : "";

                var resp = await _httpClient.GetAsync($"api/meter-readings{queryStr}");
                if (resp.IsSuccessStatusCode)
                {
                    var json = await resp.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<List<MeterReadingListItemViewModel>>(json, _jsonOptions) ?? new List<MeterReadingListItemViewModel>();

                    // Lấy ds phòng để query hiển thị tên
                    var roomResp = await _httpClient.GetAsync("api/phongtro");
                    if (roomResp.IsSuccessStatusCode)
                    {
                        var rmJson = await roomResp.Content.ReadAsStringAsync();
                        var rooms = JsonSerializer.Deserialize<List<Models.PhongTro>>(rmJson, _jsonOptions);
                        if (rooms != null)
                        {
                            foreach (var item in data)
                            {
                                var rDef = rooms.FirstOrDefault(x => x.Id == item.RoomId);
                                item.RoomName = rDef?.TenPhong ?? "N/A";
                            }
                        }
                    }

                    // For filter
                    ViewBag.CurrentRoomId = roomId;
                    ViewBag.CurrentMonth = monthYear;
                    return View(data);
                }
                TempData["Error"] = "Lỗi kết nối Server đọc Meter.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error: " + ex.Message;
            }
            return View(new List<MeterReadingListItemViewModel>());
        }

        private async Task PopSelectList(MeterReadingFormViewModel model)
        {
            var roomResp = await _httpClient.GetAsync("api/phongtro");
            if (roomResp.IsSuccessStatusCode)
            {
                var rmJson = await roomResp.Content.ReadAsStringAsync();
                var rooms = JsonSerializer.Deserialize<List<Models.PhongTro>>(rmJson, _jsonOptions);
                if (rooms != null)
                {
                    model.AvailableRooms = rooms
                        .OrderBy(r => r.TenPhong)
                        .Select(r => new SelectListItem
                        {
                            Value = r.Id.ToString(),
                            Text = $"[P.{r.TenPhong}] Tòa {r.NhaTroId} - {r.Status}"
                        });
                }
            }
        }

        public async Task<IActionResult> Create()
        {
            var model = new MeterReadingFormViewModel();
            await PopSelectList(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MeterReadingFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopSelectList(model);
                return View(model);
            }

            try
            {
                // NewIndex must be >= OldIndex
                if (model.OldElectricityIndex > 0 && model.NewElectricityIndex < model.OldElectricityIndex)
                {
                    ModelState.AddModelError("NewElectricityIndex", "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ (Rule Kế toán).");
                    await PopSelectList(model);
                    return View(model);
                }

                if (model.OldWaterIndex > 0 && model.NewWaterIndex < model.OldWaterIndex)
                {
                    ModelState.AddModelError("NewWaterIndex", "Chỉ số mới phải lớn hơn hoặc bằng chỉ số cũ (Rule Kế toán).");
                    await PopSelectList(model);
                    return View(model);
                }

                var payload = new
                {
                    RoomId = model.RoomId,
                    MonthYear = model.MonthYear,
                    OldElectricityIndex = model.OldElectricityIndex,
                    NewElectricityIndex = model.NewElectricityIndex,
                    OldWaterIndex = model.OldWaterIndex,
                    NewWaterIndex = model.NewWaterIndex,
                    Note = model.Note
                };

                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/meter-readings", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Chốt chỉ số thành công! Nếu để trống số cũ, Backend đã tự Auto-Map với tháng trước (Rule 5.1).";
                    return RedirectToAction(nameof(Index));
                }

                var err = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"BMS Data Violation Catcher: {err}";
                await PopSelectList(model);
                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"System Crash: {ex.Message}";
                await PopSelectList(model);
                return View(model);
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/meter-readings/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Đã Xóa Báo Cáo Chốt Số.";
                }
                else
                {
                    TempData["Error"] = "Cấm thực thi: Lỗi Xóa.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Exception: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Import()
        {
            return View();
        }
    }
}
