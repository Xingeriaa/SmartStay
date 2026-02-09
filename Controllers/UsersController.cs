using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace do_an_tot_nghiep.Controllers
{
    // BFF Controller - Orchestrator between Razor View and Backend API
    public class UsersController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<UsersController> _logger;

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public UsersController(IHttpClientFactory httpClientFactory, ILogger<UsersController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        #region Helpers

        private HttpClient CreateClientWithToken()
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            var token = User?.FindFirst("access_token")?.Value;
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new UnauthorizedAccessException("JWT Token not found in user claims");
            }

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        private async Task MapApiErrorsToModelState(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(content, _jsonOptions);
                if (problem?.Errors != null)
                {
                    foreach (var kv in problem.Errors)
                    {
                        foreach (var msg in kv.Value)
                        {
                            ModelState.AddModelError(kv.Key, msg);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Dữ liệu không hợp lệ từ hệ thống backend.");
                }
            }
            catch
            {
                ModelState.AddModelError("", "Lỗi nghiệp vụ từ hệ thống backend.");
            }
        }

        #endregion

        // ========================= LIST =========================
        public async Task<IActionResult> Index()
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.GetAsync("api/users");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} when calling GET api/users", response.StatusCode);
                    return View("Error");
                }

                var json = await response.Content.ReadAsStringAsync();
                var users = JsonSerializer.Deserialize<List<User>>(json, _jsonOptions) ?? new List<User>();

                return View(users);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Index");
                return View("Error");
            }
        }

        // ========================= DETAILS =========================
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.GetAsync($"api/users/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} when calling GET api/users/{Id}", response.StatusCode, id);
                    return View("Error");
                }

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

                return View(user);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Details");
                return View("Error");
            }
        }

        // ========================= CREATE =========================
        public IActionResult Create()
        {
            return View(new User());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = CreateClientWithToken();

                var jsonBody = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PostAsync("api/users", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                    response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    await MapApiErrorsToModelState(response);
                    return View(model);
                }

                _logger.LogError("API Error {Status} when POST api/users", response.StatusCode);
                return View("Error");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Create");
                return View("Error");
            }
        }

        // ========================= EDIT =========================
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.GetAsync($"api/users/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} GET api/users/{Id}", response.StatusCode, id);
                    return View("Error");
                }

                var json = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(json, _jsonOptions);

                return View(user);
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Edit(GET)");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User model)
        {
            if (id != model.Id)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var client = CreateClientWithToken();

                var jsonBody = JsonSerializer.Serialize(model, _jsonOptions);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"api/users/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                    response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    await MapApiErrorsToModelState(response);
                    return View(model);
                }

                _logger.LogError("API Error {Status} PUT api/users/{Id}", response.StatusCode, id);
                return View("Error");
            }
            catch (UnauthorizedAccessException)
            {
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Edit(POST)");
                return View("Error");
            }
        }

        // ========================= LOCK =========================
        [HttpPost]
        public async Task<IActionResult> Lock(int id)
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.PostAsync($"api/users/{id}/lock", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} POST api/users/{Id}/lock", response.StatusCode, id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Lock");
                return View("Error");
            }
        }

        // ========================= UNLOCK =========================
        [HttpPost]
        public async Task<IActionResult> Unlock(int id)
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.PostAsync($"api/users/{id}/unlock", null);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} POST api/users/{Id}/unlock", response.StatusCode, id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Unlock");
                return View("Error");
            }
        }

        // ========================= RESET PASSWORD =========================
        [HttpPost]
        public async Task<IActionResult> ResetPassword(int id, string newPassword)
        {
            try
            {
                var client = CreateClientWithToken();

                var body = JsonSerializer.Serialize(new { NewPassword = newPassword });
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"api/users/{id}/reset-password", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} POST api/users/{Id}/reset-password", response.StatusCode, id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.ResetPassword");
                return View("Error");
            }
        }

        // ========================= DELETE =========================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var client = CreateClientWithToken();
                var response = await client.DeleteAsync($"api/users/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("API Error {Status} DELETE api/users/{Id}", response.StatusCode, id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in UsersController.Delete");
                return View("Error");
            }
        }
    }
}
