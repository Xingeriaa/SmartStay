using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using do_an_tot_nghiep.Models;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class NotificationsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public NotificationsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: /Notifications
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/notifications");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var notifications = JsonSerializer.Deserialize<List<Notification>>(json, _jsonOptions);
                    return View(notifications ?? new List<Notification>());
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối tải thông báo: " + ex.Message;
            }

            return View(new List<Notification>());
        }
    }
}
