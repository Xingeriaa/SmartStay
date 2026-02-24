using do_an_tot_nghiep.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text;
using do_an_tot_nghiep.ViewModels;

namespace do_an_tot_nghiep.Controllers
{
    [RequireRole(Roles.AdminOrStaff)]
    public class TicketsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public TicketsController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        // GET: /Tickets/Index
        public async Task<IActionResult> Index(string? status, string? priority, string? search)
        {
            try
            {
                var url = "api/tickets";
                if (!string.IsNullOrWhiteSpace(status))
                    url += $"?status={Uri.EscapeDataString(status)}";

                var response = await _httpClient.GetAsync(url);
                List<TicketListItemDto> tickets = new();

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    tickets = JsonSerializer.Deserialize<List<TicketListItemDto>>(json, _jsonOptions) ?? new();
                }

                // Client-side filter priority & search
                if (!string.IsNullOrWhiteSpace(priority))
                    tickets = tickets.Where(t => t.Priority == priority).ToList();

                if (!string.IsNullOrWhiteSpace(search))
                    tickets = tickets.Where(t =>
                        t.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                        (t.Description ?? "").Contains(search, StringComparison.OrdinalIgnoreCase)
                    ).ToList();

                ViewBag.CurrentStatus = status ?? "";
                ViewBag.CurrentPriority = priority ?? "";
                ViewBag.CurrentSearch = search ?? "";

                // Stats
                ViewBag.TotalOpen = tickets.Count(t => t.Status == "Open");
                ViewBag.TotalInProgress = tickets.Count(t => t.Status == "In Progress");
                ViewBag.TotalDone = tickets.Count(t => t.Status == "Done");
                ViewBag.TotalAll = tickets.Count;

                return View(tickets);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi kết nối hệ thống: " + ex.Message;
                return View(new List<TicketListItemDto>());
            }
        }

        // GET: /Tickets/Details/5
        public async Task<IActionResult> Details(long id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/tickets/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = $"Không tìm thấy ticket #{id}.";
                    return RedirectToAction(nameof(Index));
                }

                var json = await response.Content.ReadAsStringAsync();
                var ticket = JsonSerializer.Deserialize<TicketListItemDto>(json, _jsonOptions);
                if (ticket == null) return RedirectToAction(nameof(Index));

                var vm = new TicketDetailViewModel
                {
                    Id = ticket.Id,
                    Title = ticket.Title,
                    Description = ticket.Description ?? "",
                    Status = ticket.Status,
                    Priority = ticket.Priority,
                    RoomId = ticket.RoomId,
                    RoomName = $"Phòng #{ticket.RoomId}",
                    CreatedBy = ticket.CreatedBy,
                    CreatedDate = ticket.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    AssignedToStaff = ticket.AssignedTo.HasValue ? $"Staff #{ticket.AssignedTo}" : "",
                    ImageUrls = ticket.Images?.Select(i => i.ImageUrl).ToList() ?? new()
                };

                // Populate staff dropdown
                ViewBag.Staffs = new List<SelectListItem>
                {
                    new SelectListItem { Value = "1", Text = "Nhân viên 1" },
                    new SelectListItem { Value = "2", Text = "Nhân viên 2" },
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Tickets/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Assign(long id, int staffId)
        {
            var payload = new { assignedTo = staffId };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/tickets/{id}/assign", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Đã phân công nhân viên." : "Lỗi phân công.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Tickets/UpdateStatus
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(long id, string status)
        {
            var payload = new { status };
            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync($"api/tickets/{id}/status", content);

            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? $"Cập nhật trạng thái → {status}." : "Lỗi cập nhật trạng thái.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: /Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(long id)
        {
            var response = await _httpClient.DeleteAsync($"api/tickets/{id}");
            TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
                response.IsSuccessStatusCode ? "Đã xóa ticket." : "Lỗi xóa ticket.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Tickets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int roomId, string title, string? description, string priority)
        {
            if (string.IsNullOrWhiteSpace(title) || title.Length < 5)
            {
                TempData["Error"] = "Tiêu đề phải có ít nhất 5 ký tự.";
                return View();
            }
            var payload = new
            {
                roomId,
                title,
                description,
                priority = priority ?? "Medium",
                status = "Open",
                createdBy = 1 // fallback; replace with session user
            };
            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(payload),
                System.Text.Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("api/tickets", content);
            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đã tạo phiếu sự cố thành công.";
                return RedirectToAction(nameof(Index));
            }
            TempData["Error"] = "Lỗi tạo phiếu: " + await response.Content.ReadAsStringAsync();
            return View();
        }

        // GET: /Tickets/Edit/5
        public async Task<IActionResult> Edit(long id)
        {
            var response = await _httpClient.GetAsync($"api/tickets/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Không tìm thấy ticket #{id}.";
                return RedirectToAction(nameof(Index));
            }
            var json = await response.Content.ReadAsStringAsync();
            var ticket = System.Text.Json.JsonSerializer.Deserialize<TicketListItemDto>(json, _jsonOptions);
            if (ticket == null) return RedirectToAction(nameof(Index));
            return View(ticket);
        }

        // POST: /Tickets/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, string title, string? description, string priority, string status)
        {
            var response = await _httpClient.GetAsync($"api/tickets/{id}");
            if (!response.IsSuccessStatusCode) return RedirectToAction(nameof(Index));

            var json = await response.Content.ReadAsStringAsync();
            var ticket = System.Text.Json.JsonSerializer.Deserialize<TicketListItemDto>(json, _jsonOptions);
            if (ticket == null) return RedirectToAction(nameof(Index));

            ticket.Title = title;
            ticket.Description = description;
            ticket.Priority = priority;
            ticket.Status = status;

            var payload = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(ticket),
                System.Text.Encoding.UTF8, "application/json");
            var putResp = await _httpClient.PutAsync($"api/tickets/{id}", payload);

            TempData[putResp.IsSuccessStatusCode ? "Success" : "Error"] =
                putResp.IsSuccessStatusCode ? "Đã cập nhật ticket." : "Lỗi cập nhật.";
            return RedirectToAction(nameof(Details), new { id });
        }
    }

    // ─── Internal DTOs ───────────────────────────────────────────────
    internal class TicketListItemDto
    {
        public long Id { get; set; }
        public int CreatedBy { get; set; }
        public int RoomId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public string Status { get; set; } = "Open";
        public int? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<TicketImageDto> Images { get; set; } = new();
    }

    internal class TicketImageDto
    {
        public long Id { get; set; }
        public long TicketId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}
