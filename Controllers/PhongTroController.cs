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

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class PhongTroController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PhongTroController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: PhongTro
        // Support filter by BuildingId internally
        public async Task<IActionResult> Index(int? buildingId)
        {
            try
            {
                var response = await _httpClient.GetAsync("api/phongtro");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Lỗi gọi API danh sách phòng.";
                    return View(new List<PhongTroListItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<Models.PhongTro>>(json, _jsonOptions) ?? new List<Models.PhongTro>();

                // Lọc isDeleted = 0
                var activeRooms = data.Where(p => !p.IsDeleted);

                if (buildingId.HasValue)
                {
                    activeRooms = activeRooms.Where(p => p.NhaTroId == buildingId.Value);
                    ViewBag.BuildingId = buildingId.Value;
                }

                var viewModels = activeRooms.Select(p => new PhongTroListItemViewModel
                {
                    Id = p.Id,
                    TenPhong = p.TenPhong,
                    NhaTroId = p.NhaTroId,
                    Tang = p.Tang,
                    DienTich = p.DienTich,
                    GiaPhong = p.GiaPhong,
                    SoLuongNguoi = p.SoLuongNguoi,
                    Status = p.Status
                }).OrderBy(x => x.NhaTroId).ThenBy(x => x.Tang).ThenBy(x => x.TenPhong).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return View(new List<PhongTroListItemViewModel>());
            }
        }

        // GET: PhongTro/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/phongtro/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var room = JsonSerializer.Deserialize<Models.PhongTro>(json, _jsonOptions);
                if (room == null || room.IsDeleted) return NotFound();

                var vm = new PhongTroListItemViewModel
                {
                    Id = room.Id,
                    TenPhong = room.TenPhong,
                    NhaTroId = room.NhaTroId,
                    Tang = room.Tang,
                    DienTich = room.DienTich,
                    GiaPhong = room.GiaPhong,
                    SoLuongNguoi = room.SoLuongNguoi,
                    Status = room.Status
                };
                return View(vm);
            }
            catch
            {
                TempData["Error"] = "Không tải được thông tin phòng.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: PhongTro/Create
        public IActionResult Create(int? buildingId)
        {
            var model = new PhongTroFormViewModel();
            if (buildingId.HasValue) model.NhaTroId = buildingId.Value;
            return View(model);
        }

        // POST: PhongTro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PhongTroFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/phongtro", content);

                if (response.IsSuccessStatusCode)
                {
                    // Lịch sử trạng thái phòng (RoomStatusHistory) lý tưởng nhất là bắt ở trigger DB hoặc logic API
                    TempData["Success"] = "Thêm phòng mới thành công!";
                    return RedirectToAction(nameof(Index), new { buildingId = model.NhaTroId });
                }

                TempData["Error"] = "Gửi lệnh tạo phòng thất bại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi server: {ex.Message}");
                return View(model);
            }
        }

        // GET: PhongTro/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/phongtro/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<PhongTroFormViewModel>(json, _jsonOptions);
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Không lấy được thông tin phòng.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: PhongTro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PhongTroFormViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/phongtro/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật thông tin phòng thành công!";
                    return RedirectToAction(nameof(Index), new { buildingId = model.NhaTroId });
                }

                TempData["Error"] = "Cập nhật phòng thất bại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối Backend: {ex.Message}");
                return View(model);
            }
        }

        // POST: PhongTro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/phongtro/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa phòng thành công (Soft Delete).";
                }
                else
                {
                    TempData["Error"] = "Lỗi: Không được phép xóa phòng nếu phòng đang có người thuê / có hợp đồng đang active.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
