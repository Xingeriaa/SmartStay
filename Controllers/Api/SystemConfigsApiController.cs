using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý cấu hình hệ thống (ngày chốt sổ, hạn đóng tiền, giá mặc định...)
    /// </summary>
    [ApiController]
    [Route("api/system-configs")]
    public class SystemConfigsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SystemConfigsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Lấy toàn bộ cấu hình hệ thống.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<SystemConfig>>> GetAll()
        {
            return await _context.SystemConfigs.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Lấy cấu hình theo khóa.
        /// </summary>
        [HttpGet("{key}")]
        public async Task<ActionResult<SystemConfig>> GetByKey(string key)
        {
            var config = await _context.SystemConfigs.FindAsync(key);
            if (config == null) return NotFound();
            return config;
        }

        /// <summary>
        /// Tạo hoặc cập nhật cấu hình theo khóa (upsert).
        /// </summary>
        [HttpPut("{key}")]
        public async Task<ActionResult<SystemConfig>> Upsert(string key, SystemConfig input)
        {
            if (key != input.Key) return BadRequest("ConfigKey không khớp.");

            var existing = await _context.SystemConfigs.FindAsync(key);
            if (existing == null)
            {
                _context.SystemConfigs.Add(input);
            }
            else
            {
                existing.Value = input.Value;
                existing.Description = input.Description;
            }

            await _context.SaveChangesAsync();
            return Ok(input);
        }

        /// <summary>
        /// Xóa cấu hình theo khóa.
        /// </summary>
        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            var existing = await _context.SystemConfigs.FindAsync(key);
            if (existing == null) return NotFound();

            _context.SystemConfigs.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
