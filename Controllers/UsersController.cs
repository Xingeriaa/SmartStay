using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using do_an_tot_nghiep.ViewModels;
using do_an_tot_nghiep.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Linq;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.Admin)]
    public class UsersController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public UsersController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/users");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Không thể tải danh sách tài khoản hệ thống.";
                    return View(new List<UserListItemViewModel>());
                }

                var json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<User>>(json, _jsonOptions) ?? new List<User>();

                var viewModels = data.Where(u => !u.IsDeleted).Select(u => new UserListItemViewModel
                {
                    Id = u.Id,
                    Username = u.Username,
                    FullName = u.FullName,
                    RoleName = u.Role?.RoleName ?? "Unknown",
                    IsActive = u.IsActive,
                    LastLoginAt = u.LastLoginAt
                }).OrderByDescending(x => x.Id).ToList();

                return View(viewModels);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi kết nối Server: {ex.Message}";
                return View(new List<UserListItemViewModel>());
            }
        }

        // GET: Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

                if (user == null || user.IsDeleted) return NotFound();

                return View(new UserListItemViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    RoleName = user.Role?.RoleName ?? "Unknown",
                    IsActive = user.IsActive,
                    LastLoginAt = user.LastLoginAt
                });
            }
            catch
            {
                TempData["Error"] = "Đã xảy ra lỗi khi tải hồ sơ tài khoản.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Users/Create
        public IActionResult Create()
        {
            return View(new UserFormViewModel());
        }

        // POST: Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Mật khẩu không được để trống khi tạo mới.");
                    return View(model);
                }

                var apiModel = new User
                {
                    Username = model.Username,
                    Password = model.Password, // Lưu ý: Ở production cần mã hóa BCrypt
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone,
                    RoleName = model.RoleName,
                    IsActive = model.IsActive
                };

                var content = new StringContent(JsonSerializer.Serialize(apiModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/users", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Quản lý đã tạo tài khoản thành công!";
                    return RedirectToAction(nameof(Index));
                }

                TempData["Error"] = "Thất bại khi gửi lệnh tạo lên API.";
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi kết nối Backend: {ex.Message}");
                return View(model);
            }
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/users/{id}");
                if (!response.IsSuccessStatusCode) return NotFound();

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

                if (user == null || user.IsDeleted) return NotFound();

                var formModel = new UserFormViewModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    RoleName = user.Role?.RoleName ?? "Staff",
                    IsActive = user.IsActive,
                    Password = "" // Khuyên trống để báo hiệu không đổi pass
                };

                return View(formModel);
            }
            catch
            {
                TempData["Error"] = "Không lấy được thông tin tài khoản.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserFormViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            try
            {
                // Gọi cấu hình update profile
                var profileRequest = new
                {
                    Username = model.Username,
                    Email = model.Email,
                    Role = model.RoleName
                };

                var contentProfile = new StringContent(JsonSerializer.Serialize(profileRequest), Encoding.UTF8, "application/json");
                var responseProfile = await _httpClient.PutAsync($"api/users/{id}/profile", contentProfile);

                if (!responseProfile.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Lỗi khi cập nhật Profile/RBAC.";
                    return View(model);
                }

                // Nếu có nhập pass thì đổi pass
                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var passReq = new { NewPassword = model.Password };
                    var contentPass = new StringContent(JsonSerializer.Serialize(passReq), Encoding.UTF8, "application/json");
                    await _httpClient.PostAsync($"api/users/{id}/reset-password", contentPass);
                }

                // Xử lý Active/Lock
                if (model.IsActive)
                    await _httpClient.PostAsync($"api/users/{id}/unlock", null);
                else
                    await _httpClient.PostAsync($"api/users/{id}/lock", null);

                TempData["Success"] = "Cập nhật tài khoản, RBAC thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi xử lý API: {ex.Message}");
                return View(model);
            }
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/users/{id}");
                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Xóa Soft Delete tài khoản thành công.";
                }
                else
                {
                    TempData["Error"] = "Có lỗi xảy ra, hệ thống từ chối xóa.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Lỗi mất kết nối truy hồi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
