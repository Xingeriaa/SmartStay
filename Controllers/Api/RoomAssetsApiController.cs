using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quan ly trang thiet bi trong phong.
    /// </summary>
    [ApiController]
    [Route("api/room-assets")]
    public class RoomAssetsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoomAssetsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sach thiet bi theo phong (roomId co the bo trong).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RoomAsset>>> GetAll([FromQuery] int? roomId)
        {
            var roomAssets = _context.RoomAssets.AsNoTracking();
            if (roomId.HasValue)
            {
                roomAssets = roomAssets.Where(x => x.RoomId == roomId.Value);
            }

            var items = await (from ra in roomAssets
                               join a in _context.Assets.AsNoTracking() on ra.AssetId equals a.Id into aj
                               from a in aj.DefaultIfEmpty()
                               orderby ra.Id descending
                               select new RoomAsset
                               {
                                   Id = ra.Id,
                                   RoomId = ra.RoomId,
                                   AssetId = ra.AssetId,
                                   AssetName = a != null ? a.AssetName : string.Empty,
                                   SerialNumber = ra.SerialNumber,
                                   Quantity = ra.Quantity,
                                   Status = ra.Status,
                                   PurchaseDate = ra.PurchaseDate,
                                   WarrantyExpiry = ra.WarrantyExpiry,
                                   ConditionScore = ra.ConditionScore,
                                   LastMaintenanceDate = ra.LastMaintenanceDate,
                                   MaintenanceCycleMonths = ra.MaintenanceCycleMonths,
                                   IsUnderWarranty = ra.IsUnderWarranty,
                                   LocationNote = ra.LocationNote,
                                   CreatedAt = ra.CreatedAt
                               }).ToListAsync();

            return items;
        }

        /// <summary>
        /// Lay thiet bi theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomAsset>> GetById(int id)
        {
            var asset = await (from ra in _context.RoomAssets.AsNoTracking()
                               where ra.Id == id
                               join a in _context.Assets.AsNoTracking() on ra.AssetId equals a.Id into aj
                               from a in aj.DefaultIfEmpty()
                               select new RoomAsset
                               {
                                   Id = ra.Id,
                                   RoomId = ra.RoomId,
                                   AssetId = ra.AssetId,
                                   AssetName = a != null ? a.AssetName : string.Empty,
                                   SerialNumber = ra.SerialNumber,
                                   Quantity = ra.Quantity,
                                   Status = ra.Status,
                                   PurchaseDate = ra.PurchaseDate,
                                   WarrantyExpiry = ra.WarrantyExpiry,
                                   ConditionScore = ra.ConditionScore,
                                   LastMaintenanceDate = ra.LastMaintenanceDate,
                                   MaintenanceCycleMonths = ra.MaintenanceCycleMonths,
                                   IsUnderWarranty = ra.IsUnderWarranty,
                                   LocationNote = ra.LocationNote,
                                   CreatedAt = ra.CreatedAt
                               }).FirstOrDefaultAsync();

            if (asset == null) return NotFound();
            return asset;
        }

        /// <summary>
        /// Tao thiet bi moi.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create(RoomAsset asset)
        {
            var resolvedAssetId = await ResolveAssetIdAsync(asset.AssetId, asset.AssetName);
            if (resolvedAssetId == null)
            {
                return BadRequest("AssetId hoac AssetName la bat buoc.");
            }

            // BR-A3: SerialNumber UNIQUE toàn hệ thống
            if (!string.IsNullOrWhiteSpace(asset.SerialNumber))
            {
                var serialExists = await _context.RoomAssets.AnyAsync(a => a.SerialNumber == asset.SerialNumber);
                if (serialExists)
                    return BadRequest(new { message = $"Mã Serial '{asset.SerialNumber}' đã tồn tại trong hệ thống." });
            }

            asset.AssetId = resolvedAssetId.Value;
            _context.RoomAssets.Add(asset);
            await _context.SaveChangesAsync();

            var created = await GetById(asset.Id);
            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, created.Value);
        }

        /// <summary>
        /// Cap nhat thiet bi.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, RoomAsset asset)
        {
            if (id != asset.Id) return BadRequest();

            var existing = await _context.RoomAssets.FindAsync(id);
            if (existing == null) return NotFound();

            var resolvedAssetId = await ResolveAssetIdAsync(asset.AssetId, asset.AssetName);
            if (resolvedAssetId == null)
            {
                return BadRequest("AssetId hoac AssetName la bat buoc.");
            }

            // BR-A3: SerialNumber UNIQUE toàn hệ thống
            if (!string.IsNullOrWhiteSpace(asset.SerialNumber))
            {
                var serialExists = await _context.RoomAssets.AnyAsync(a => a.SerialNumber == asset.SerialNumber && a.Id != id);
                if (serialExists)
                    return BadRequest(new { message = $"Mã Serial '{asset.SerialNumber}' đã tồn tại trong hệ thống." });
            }

            existing.RoomId = asset.RoomId;
            existing.AssetId = resolvedAssetId.Value;
            existing.SerialNumber = asset.SerialNumber;
            existing.Quantity = asset.Quantity;
            existing.Status = asset.Status;
            existing.PurchaseDate = asset.PurchaseDate;
            existing.WarrantyExpiry = asset.WarrantyExpiry;
            existing.ConditionScore = asset.ConditionScore;
            existing.LastMaintenanceDate = asset.LastMaintenanceDate;
            existing.MaintenanceCycleMonths = asset.MaintenanceCycleMonths;
            existing.IsUnderWarranty = asset.IsUnderWarranty;
            existing.LocationNote = asset.LocationNote;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xoa thiet bi.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.RoomAssets.FindAsync(id);
            if (asset == null) return NotFound();

            // BR-A1: Không xóa Asset nếu có MaintenanceHistory.
            var hasMaintenanceLog = await _context.AssetMaintenanceLogs.AnyAsync(l => l.RoomAssetId == id);
            if (hasMaintenanceLog)
                return BadRequest(new { message = "Không thể xóa tài sản vì đã có lịch sử bảo trì (Maintenance Log)." });

            _context.RoomAssets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task<int?> ResolveAssetIdAsync(int assetId, string? assetName)
        {
            if (assetId > 0)
            {
                var exists = await _context.Assets.AnyAsync(x => x.Id == assetId);
                return exists ? assetId : null;
            }

            if (string.IsNullOrWhiteSpace(assetName))
            {
                return null;
            }

            var normalized = assetName.Trim();
            var existing = await _context.Assets.FirstOrDefaultAsync(x => x.AssetName == normalized);
            if (existing != null)
            {
                return existing.Id;
            }

            var newAsset = new Asset
            {
                AssetName = normalized,
                CreatedAt = DateTime.UtcNow
            };

            _context.Assets.Add(newAsset);
            await _context.SaveChangesAsync();
            return newAsset.Id;
        }
    }
}
