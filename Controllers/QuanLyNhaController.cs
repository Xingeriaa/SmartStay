using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;
using System.Linq;

namespace do_an_tot_nghiep.Controllers
{
    /// <summary>
    /// QuanLyNha Controller – MVC controller serving Views/QuanLyNha/*.cshtml
    /// All data fetched from backend REST API via HttpClient "ApiClient".
    /// </summary>
    [RequireRole(Roles.AdminOrStaff)]
    public class QuanLyNhaController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public QuanLyNhaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // ─────────────────────────────────────────────────────────────────────
        // INDEX – danh sách tất cả nhà trọ
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/nhatro");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải danh sách tòa nhà từ máy chủ.";
                    return View(new List<NhaTro>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<NhaTro>>(json, _jsonOptions)
                           ?? new List<NhaTro>();

                return View(data.Where(n => !n.IsDeleted).ToList());
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
                return View(new List<NhaTro>());
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // DETAILS – xem chi tiết + danh sách phòng trong nhà
        // ─────────────────────────────────────────────────────────────────────
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var nhaTroTask = _httpClient.GetAsync($"api/nhatro/{id}");
                var phongTask = _httpClient.GetAsync("api/phongtro");

                await Task.WhenAll(nhaTroTask, phongTask);

                if (!nhaTroTask.Result.IsSuccessStatusCode) return NotFound();

                var nhaTroJson = await nhaTroTask.Result.Content.ReadAsStringAsync();
                var nhaTro = JsonSerializer.Deserialize<NhaTro>(nhaTroJson, _jsonOptions);

                if (nhaTro == null) return NotFound();

                if (phongTask.Result.IsSuccessStatusCode)
                {
                    var phongJson = await phongTask.Result.Content.ReadAsStringAsync();
                    var allPhong = JsonSerializer.Deserialize<List<PhongTro>>(phongJson, _jsonOptions) ?? new();
                    ViewBag.DanhSachPhong = allPhong.Where(p => p.NhaTroId == id && !p.IsDeleted).ToList();
                }

                return View(nhaTro);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi khi tải chi tiết: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // CREATE – GET
        // ─────────────────────────────────────────────────────────────────────
        public IActionResult Create()
        {
            return View(new NhaTro());
        }

        // ─────────────────────────────────────────────────────────────────────
        // CREATE – POST
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaTro model, int[] DichVuSelect)
        {
            if (!ModelState.IsValid) return View(model);

            // Lưu danh sách dịch vụ
            if (DichVuSelect.Length > 0)
                model.DanhSachDichVu = string.Join(",", DichVuSelect);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/nhatro", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tạo tòa nhà mới thành công!";
                    return RedirectToAction(nameof(Index));
                }

                var error = await response.Content.ReadAsStringAsync();
                TempData["Error"] = $"Lỗi lưu dữ liệu: {error}";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối: {ex.Message}");
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // EDIT – GET
        // ─────────────────────────────────────────────────────────────────────
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/nhatro/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<NhaTro>(json, _jsonOptions);
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối khi tải dữ liệu nhà.";
                return RedirectToAction(nameof(Index));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // EDIT – POST
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaTro model, int[] DichVuSelect)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            if (DichVuSelect.Length > 0)
                model.DanhSachDichVu = string.Join(",", DichVuSelect);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/nhatro/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật tòa nhà thành công!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Cập nhật thất bại. Vui lòng thử lại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối: {ex.Message}");
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE – POST  (single, from hidden form in Index)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/nhatro/{id}");
                TempData[response.IsSuccessStatusCode ? "Success" : "Error"] = response.IsSuccessStatusCode
                    ? "Xóa tòa nhà thành công."
                    : "Xóa thất bại. Tòa nhà có thể đang chứa phòng đang hoạt động.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        // ─────────────────────────────────────────────────────────────────────
        // DELETE SELECTED – POST (bulk, AJAX)
        // ─────────────────────────────────────────────────────────────────────
        [HttpPost]
        public async Task<IActionResult> DeleteSelected(int[] ids)
        {
            if (ids == null || ids.Length == 0)
                return Json(new { success = false, message = "Không có mục nào được chọn." });

            int ok = 0, fail = 0;
            foreach (var id in ids)
            {
                var resp = await _httpClient.DeleteAsync($"api/nhatro/{id}");
                if (resp.IsSuccessStatusCode) ok++; else fail++;
            }

            return Json(new
            {
                success = ok > 0,
                message = fail == 0
                    ? $"Đã xóa {ok} tòa nhà thành công."
                    : $"Xóa {ok} thành công, {fail} thất bại (đang có phòng hoạt động)."
            });
        }
    }
}
