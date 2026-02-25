using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using do_an_tot_nghiep.Models;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class SystemConfigsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public SystemConfigsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: /SystemConfigs
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/system-configs");
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var configs = JsonSerializer.Deserialize<List<SystemConfig>>(json, _jsonOptions);
                    ViewBag.Configs = configs;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối Server: " + ex.Message;
            }

            ViewBag.CanEdit = true;
            return View();
        }
    }
}
