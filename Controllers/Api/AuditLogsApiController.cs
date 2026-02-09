using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Tra cứu nhật ký hoạt động (Audit Log).
    /// </summary>
    [ApiController]
    [Route("api/audit-logs")]
    public class AuditLogsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AuditLogsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy danh sách nhật ký, hỗ trợ lọc theo userId hoặc entityName.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<AuditLog>>> GetAll([FromQuery] int? userId, [FromQuery] string? entityName)
        {
            var query = _context.AuditLogs.AsNoTracking();

            if (userId.HasValue)
            {
                query = query.Where(x => x.UserId == userId.Value);
            }

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(x => x.EntityName == entityName);
            }

            return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
        }

        /// <summary>
        /// Ghi nhận nhật ký (dùng khi backend cần ghi log thủ công).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AuditLog>> Create(AuditLog log)
        {
            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAll), new { id = log.Id }, log);
        }
    }
}
