using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý phân quyền theo tòa nhà/phòng/chức năng (RBAC mở rộng).
    /// </summary>
    [ApiController]
    [Route("api/permissions")]
    public class UserPermissionsApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UserPermissionsApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách quyền, có thể lọc theo UserId.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UserPermission>>> GetAll([FromQuery] int? userId)
        {
            var query = _context.UserPermissions.AsNoTracking();
            if (userId.HasValue)
            {
                query = query.Where(p => p.UserId == userId.Value);
            }
            return await query.ToListAsync();
        }

        /// <summary>
        /// Lấy quyền theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserPermission>> GetById(int id)
        {
            var permission = await _context.UserPermissions.FindAsync(id);
            if (permission == null) return NotFound();
            return permission;
        }

        /// <summary>
        /// Tạo quyền mới.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UserPermission>> Create(UserPermission permission)
        {
            _context.UserPermissions.Add(permission);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = permission.Id }, permission);
        }

        /// <summary>
        /// Cập nhật quyền.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, UserPermission permission)
        {
            if (id != permission.Id) return BadRequest();

            _context.Entry(permission).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa quyền.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var permission = await _context.UserPermissions.FindAsync(id);
            if (permission == null) return NotFound();

            _context.UserPermissions.Remove(permission);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
