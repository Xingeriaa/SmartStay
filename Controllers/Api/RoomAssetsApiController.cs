using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý trang thiết bị trong phòng.
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
        /// Danh sách thiết bị theo phòng (roomId có thể bỏ trống).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RoomAsset>>> GetAll([FromQuery] int? roomId)
        {
            var query = _context.RoomAssets.AsNoTracking();
            if (roomId.HasValue)
            {
                query = query.Where(x => x.RoomId == roomId.Value);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Lấy thiết bị theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<RoomAsset>> GetById(int id)
        {
            var asset = await _context.RoomAssets.FindAsync(id);
            if (asset == null) return NotFound();
            return asset;
        }

        /// <summary>
        /// Tạo thiết bị mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RoomAsset>> Create(RoomAsset asset)
        {
            _context.RoomAssets.Add(asset);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = asset.Id }, asset);
        }

        /// <summary>
        /// Cập nhật thiết bị.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, RoomAsset asset)
        {
            if (id != asset.Id) return BadRequest();

            _context.Entry(asset).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa thiết bị.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var asset = await _context.RoomAssets.FindAsync(id);
            if (asset == null) return NotFound();

            _context.RoomAssets.Remove(asset);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
