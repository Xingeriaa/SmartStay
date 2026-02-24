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
    public class NhaTroController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public NhaTroController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: NhaTro
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/nhatro");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải danh sách Tòa Nhà từ API Server.";
                    return View(new List<NhaTroListItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<NhaTroListItemViewModel>>(json, _jsonOptions) ?? new List<NhaTroListItemViewModel>();

                return View(data);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối Server: {ex.Message}";
                return View(new List<NhaTroListItemViewModel>());
            }
        }

        // GET: NhaTro/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/nhatro/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<NhaTroFormViewModel>(json, _jsonOptions);
                return View(data);
            }
            catch
            {
                TempData["Error"] = "Đã xảy ra lỗi khi lấy dữ liệu từ API.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: NhaTro/Create
        public IActionResult Create()
        {
            return View(new NhaTroFormViewModel());
        }

        // POST: NhaTro/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NhaTroFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/nhatro", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Tạo Tòa Nhà thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Có lỗi xảy ra khi lưu trên API.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối tới Server Backend: {ex.Message}");
                return View(model);
            }
        }

        // GET: NhaTro/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/nhatro/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var model = JsonSerializer.Deserialize<NhaTroFormViewModel>(json, _jsonOptions);
                return View(model);
            }
            catch
            {
                TempData["Error"] = "Lỗi kết nối API Server.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: NhaTro/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, NhaTroFormViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"api/nhatro/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Cập nhật Tòa Nhà thành công!";
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = "Cập nhật thất bại. Xin vui lòng thử lại.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối: {ex.Message}");
                return View(model);
            }
        }

        // POST: NhaTro/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/nhatro/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa Tòa Nhà thành công (Dữ liệu đã được ẩn an toàn).";
                }
                else
                {
                    TempData["Error"] = "Xóa thất bại. Tòa nhà này có thể đang chứa phòng đang thuê.";
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
