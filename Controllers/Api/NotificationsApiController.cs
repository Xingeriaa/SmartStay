using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý thông báo và trạng thái đã đọc.
    /// </summary>
    [ApiController]
    [Route("api/notifications")]
    public class NotificationsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách thông báo (lọc theo userId).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Notification>>> GetAll([FromQuery] int? userId)
        {
            var query = _context.Notifications.AsNoTracking();
            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }
            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Tạo thông báo mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Notification>> Create(Notification notification)
        {
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = notification.Id }, notification);
        }

        /// <summary>
        /// Đánh dấu đã đọc.
        /// </summary>
        [HttpPost("{id:int}/read")]
        public async Task<IActionResult> MarkRead(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa thông báo.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null) return NotFound();

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
