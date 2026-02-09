using do_an_tot_nghiep.Models;
using do_an_tot_nghiep.Services;
using Microsoft.AspNetCore.Mvc;

namespace do_an_tot_nghiep.Controllers
{

    public class RoomAssetsController : Controller
    {
        private readonly ILogger<RoomAssetsController> _logger;
        private readonly IRoomAssetsService _roomAssetsService;

        public RoomAssetsController(
            ILogger<RoomAssetsController> logger,
            IRoomAssetsService roomAssetsService)
        {
            _logger = logger;
            _roomAssetsService = roomAssetsService;
        }

        #region ===== Enums =====

        public enum AssetStatus
        {
            New = 1,
            Damaged = 2,
            Broken = 3,
            Disposed = 4
        }

        #endregion

        #region ===== Helpers =====

        private string? ResolveAccessToken()
        {
            return User.FindFirst("access_token")?.Value
                   ?? Request.Cookies["access_token"];
        }

        #endregion

        // =============================
        // LIST
        // =============================
        public async Task<IActionResult> Index(int? roomId)
        {
            try
            {
                var result = await _roomAssetsService.GetAssetsAsync(ResolveAccessToken(), roomId);
                if (result.RequiresLogin) return RedirectToAction("Login", "Account");

                if (!result.Success)
                {
                    _logger.LogError("RoomAssets Index failed. Error: {Error}", result.ErrorMessage);
                    return View("Error");
                }

                return View("RoomAssetsList", result.Assets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading RoomAssets list");
                return View("Error");
            }
        }

        // =============================
        // DETAIL
        // =============================
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var result = await _roomAssetsService.GetAssetAsync(ResolveAccessToken(), id);
                if (result.RequiresLogin) return RedirectToAction("Login", "Account");
                if (result.NotFound) return NotFound();

                if (!result.Success)
                {
                    _logger.LogError("RoomAssets Detail failed. Error: {Error}", result.ErrorMessage);
                    return View("Error");
                }

                return View("RoomAssetDetail", result.Asset);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading RoomAsset detail. Id={Id}", id);
                return View("Error");
            }
        }

        // =============================
        // CREATE (GET)
        // =============================
        public IActionResult Create(int roomId)
        {
            var model = new RoomAsset
            {
                RoomId = roomId,
                Status = (int)AssetStatus.New
            };

            ViewBag.StatusList = Enum.GetValues(typeof(AssetStatus))
                .Cast<AssetStatus>()
                .Select(x => new { Id = (int)x, Name = x.ToString() })
                .ToList();

            return View("RoomAssetCreate", model);
        }

        // =============================
        // CREATE (POST)
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoomAsset model)
        {
            if (!ModelState.IsValid)
                return View("RoomAssetCreate", model);

            try
            {
                var result = await _roomAssetsService.CreateAsync(ResolveAccessToken(), model);
                if (result.RequiresLogin) return RedirectToAction("Login", "Account");

                if (result.ValidationErrors != null)
                {
                    foreach (var kv in result.ValidationErrors)
                    {
                        foreach (var msg in kv.Value)
                        {
                            ModelState.AddModelError(kv.Key, msg);
                        }
                    }
                    return View("RoomAssetCreate", model);
                }

                if (!result.Success)
                {
                    _logger.LogError("Create RoomAsset failed. Error: {Error}", result.ErrorMessage);
                    return View("Error");
                }

                return RedirectToAction(nameof(Index), new { roomId = model.RoomId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating RoomAsset");
                return View("Error");
            }
        }

        // =============================
        // DELETE
        // =============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int roomId)
        {
            try
            {
                var result = await _roomAssetsService.DeleteAsync(ResolveAccessToken(), id);
                if (result.RequiresLogin) return RedirectToAction("Login", "Account");

                if (!result.Success)
                {
                    _logger.LogError("Delete RoomAsset failed. Error: {Error}", result.ErrorMessage);
                    return View("Error");
                }

                return RedirectToAction(nameof(Index), new { roomId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting RoomAsset. Id={Id}", id);
                return View("Error");
            }
        }
    }
}
