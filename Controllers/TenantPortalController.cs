using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace do_an_tot_nghiep.Controllers
{
    /// <summary>
    /// Tenant Portal — trang dành riêng cho Người Thuê.
    /// Hiển thị danh sách tòa nhà và phòng trống để tham khảo.
    /// </summary>
    [RequireRole(Roles.Tenant)]
    public class TenantPortalController : Controller
    {
        private readonly HttpClient _http;
        private readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        public TenantPortalController(IHttpClientFactory factory)
        {
            _http = factory.CreateClient("ApiClient");
        }

        // GET /TenantPortal
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách tòa nhà
            dynamic? buildings = null;
            try
            {
                var res = await _http.GetAsync("api/nha-tro");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    buildings = JsonSerializer.Deserialize<List<dynamic>>(json, _json);
                }
            }
            catch { /* API offline — hiện empty state */ }

            ViewBag.Buildings = buildings;
            ViewBag.UserName = HttpContext.Session.GetString("FullName")
                             ?? HttpContext.Session.GetString("UserName");
            return View();
        }

        // GET /TenantPortal/Rooms?buildingId=1
        public async Task<IActionResult> Rooms(int buildingId)
        {
            dynamic? rooms = null;
            dynamic? building = null;
            try
            {
                var res = await _http.GetAsync($"api/nha-tro/{buildingId}");
                if (res.IsSuccessStatusCode)
                {
                    var json = await res.Content.ReadAsStringAsync();
                    building = JsonSerializer.Deserialize<dynamic>(json, _json);
                }
                var res2 = await _http.GetAsync($"api/phongtro?nhaTroId={buildingId}");
                if (res2.IsSuccessStatusCode)
                {
                    var json2 = await res2.Content.ReadAsStringAsync();
                    rooms = JsonSerializer.Deserialize<List<dynamic>>(json2, _json);
                }
            }
            catch { }

            ViewBag.Rooms = rooms;
            ViewBag.Building = building;
            ViewBag.BuildingId = buildingId;
            return View();
        }
    }
}
