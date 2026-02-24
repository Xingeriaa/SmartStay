using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using do_an_tot_nghiep.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class KhachThueController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public KhachThueController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: KhachThue
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/khachthue");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải danh sách khách thuê.";
                    return View(new List<KhachThueListItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<KhachThueListItemViewModel>>(json, _jsonOptions) ?? new List<KhachThueListItemViewModel>();

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi hệ thống: {ex.Message}";
                return View(new List<KhachThueListItemViewModel>());
            }
        }

        // GET: KhachThue/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/khachthue/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<KhachThueFormViewModel>(json, _jsonOptions);
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Lỗi khi lấy dữ liệu khách thuê.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: KhachThue/Create
        public IActionResult Create()
        {
            return View(new KhachThueFormViewModel());
        }

        // POST: KhachThue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(KhachThueFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/khachthue", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Thêm cư dân thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Thêm cư dân thất bại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi server: {ex.Message}");
                return View(model);
            }
        }

        // GET: KhachThue/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/khachthue/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<KhachThueFormViewModel>(json, _jsonOptions);
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối API.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: KhachThue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, KhachThueFormViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/khachthue/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật cư dân thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Cập nhật thất bại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi API: {ex.Message}");
                return View(model);
            }
        }

        // POST: KhachThue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/khachthue/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa (Soft Delete) khách thuê thành công!";
                }
                else
                {
                    TempData["Error"] = "Không thể xóa khách thuê do còn hợp đồng hoặc lỗi server.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
