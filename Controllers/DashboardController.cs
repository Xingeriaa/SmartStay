using do_an_tot_nghiep.Filters;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public DashboardController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index(string? monthYear)
        {
            if (string.IsNullOrEmpty(monthYear))
                monthYear = DateTime.Now.ToString("MM/yyyy");

            var url = $"api/dashboard/summary?monthYear={monthYear}";
            var resp = await _httpClient.GetAsync(url);

            var vm = new ViewModels.DashboardViewModel();
            if (resp.IsSuccessStatusCode)
            {
                var json = await resp.Content.ReadAsStringAsync();
                vm = JsonSerializer.Deserialize<ViewModels.DashboardViewModel>(json, _jsonOptions) ?? new ViewModels.DashboardViewModel();
            }

            ViewBag.CurrentMonth = monthYear;
            return View(vm);
        }

        // Báo cáo (Export Excel giả lập demo luồng)
        public IActionResult ExportExcel(string monthYear)
        {
            // Tích hợp EPPlus / ClosedXML thực tế ở Core, mock tải file return file stream
            return File(new byte[0], "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"BaoCao_{monthYear.Replace("/", "")}.xlsx");
        }
    }
}
