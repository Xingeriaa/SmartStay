using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace do_an_tot_nghiep.Services
{
    public interface IRoomAssetsService
    {
        Task<RoomAssetsListResult> GetAssetsAsync(string? token, int? roomId);
        Task<RoomAssetResult> GetAssetAsync(string? token, int id);
        Task<RoomAssetCreateResult> CreateAsync(string? token, RoomAsset model);
        Task<RoomAssetUpdateResult> UpdateAsync(string? token, RoomAsset model);
        Task<RoomAssetDeleteResult> DeleteAsync(string? token, int id);
    }

    public sealed record RoomAssetsListResult(bool Success, bool RequiresLogin, List<RoomAsset> Assets, string? ErrorMessage);
    public sealed record RoomAssetResult(bool Success, bool RequiresLogin, bool NotFound, RoomAsset? Asset, string? ErrorMessage);
    public sealed record RoomAssetCreateResult(bool Success, bool RequiresLogin, IDictionary<string, string[]>? ValidationErrors, string? ErrorMessage);
    public sealed record RoomAssetUpdateResult(bool Success, bool RequiresLogin, string? ErrorMessage);
    public sealed record RoomAssetDeleteResult(bool Success, bool RequiresLogin, string? ErrorMessage);

    public class RoomAssetsService : IRoomAssetsService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RoomAssetsService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<RoomAssetsListResult> GetAssetsAsync(string? token, int? roomId)
        {
            var client = CreateClient(token, out var requiresLogin);
            if (requiresLogin)
            {
                return new RoomAssetsListResult(false, true, new List<RoomAsset>(), null);
            }

            var query = new Dictionary<string, string?>();
            if (roomId.HasValue) query.Add("roomId", roomId.Value.ToString());

            var url = QueryHelpers.AddQueryString($"{ApiBaseUrl}/api/room-assets", query);

            var response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return new RoomAssetsListResult(false, false, new List<RoomAsset>(), $"API failed with status {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var assets = JsonSerializer.Deserialize<List<RoomAsset>>(json, _jsonOptions) ?? new List<RoomAsset>();

            return new RoomAssetsListResult(true, false, assets, null);
        }

        public async Task<RoomAssetResult> GetAssetAsync(string? token, int id)
        {
            var client = CreateClient(token, out var requiresLogin);
            if (requiresLogin)
            {
                return new RoomAssetResult(false, true, false, null, null);
            }

            var response = await client.GetAsync($"{ApiBaseUrl}/api/room-assets/{id}");

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return new RoomAssetResult(false, false, true, null, null);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new RoomAssetResult(false, false, false, null, $"API failed with status {response.StatusCode}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var asset = JsonSerializer.Deserialize<RoomAsset>(json, _jsonOptions);

            return new RoomAssetResult(true, false, false, asset, null);
        }

        public async Task<RoomAssetCreateResult> CreateAsync(string? token, RoomAsset model)
        {
            var client = CreateClient(token, out var requiresLogin);
            if (requiresLogin)
            {
                return new RoomAssetCreateResult(false, true, null, null);
            }

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{ApiBaseUrl}/api/room-assets", content);

            if (response.StatusCode == HttpStatusCode.BadRequest ||
                response.StatusCode == HttpStatusCode.UnprocessableEntity)
            {
                var errJson = await response.Content.ReadAsStringAsync();
                var errors = ParseValidationErrors(errJson);
                return new RoomAssetCreateResult(false, false, errors, null);
            }

            if (!response.IsSuccessStatusCode)
            {
                return new RoomAssetCreateResult(false, false, null, $"API failed with status {response.StatusCode}");
            }

            return new RoomAssetCreateResult(true, false, null, null);
        }

        public async Task<RoomAssetUpdateResult> UpdateAsync(string? token, RoomAsset model)
        {
            var client = CreateClient(token, out var requiresLogin);
            if (requiresLogin) return new RoomAssetUpdateResult(false, true, null);

            var jsonBody = JsonSerializer.Serialize(model);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{ApiBaseUrl}/api/room-assets/{model.Id}", content);
            if (!response.IsSuccessStatusCode)
                return new RoomAssetUpdateResult(false, false, $"API failed with status {response.StatusCode}");

            return new RoomAssetUpdateResult(true, false, null);
        }

        public async Task<RoomAssetDeleteResult> DeleteAsync(string? token, int id)
        {
            var client = CreateClient(token, out var requiresLogin);
            if (requiresLogin)
            {
                return new RoomAssetDeleteResult(false, true, null);
            }

            var response = await client.DeleteAsync($"{ApiBaseUrl}/api/room-assets/{id}");
            if (!response.IsSuccessStatusCode)
            {
                return new RoomAssetDeleteResult(false, false, $"API failed with status {response.StatusCode}");
            }

            return new RoomAssetDeleteResult(true, false, null);
        }

        private HttpClient CreateClient(string? token, out bool requiresLogin)
        {
            var client = _httpClientFactory.CreateClient("BackendApi");

            if (!string.IsNullOrWhiteSpace(token))
            {
                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }

            requiresLogin = false;
            return client;
        }

        private string ApiBaseUrl => _configuration["ApiBaseUrl"] ?? string.Empty;

        private IDictionary<string, string[]> ParseValidationErrors(string jsonError)
        {
            try
            {
                var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(jsonError, _jsonOptions);
                return problem?.Errors ?? new Dictionary<string, string[]> { { string.Empty, new[] { "Dữ liệu không hợp lệ." } } };
            }
            catch
            {
                return new Dictionary<string, string[]> { { string.Empty, new[] { "Dữ liệu không hợp lệ." } } };
            }
        }
    }
}
