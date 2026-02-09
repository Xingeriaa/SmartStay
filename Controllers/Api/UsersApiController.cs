using do_an_tot_nghiep.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Controllers.Api
{
    /// <summary>
    /// Quản lý người dùng và tài khoản hệ thống.
    /// </summary>
    [ApiController]
    [Route("api/users")]
    public class UsersApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Danh sách người dùng (kèm vai trò).
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAll()
        {
            return await _context.Users.Include(u => u.Role).ToListAsync();
        }

        /// <summary>
        /// Lấy người dùng theo id.
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<User>> GetById(int id)
        {
            var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();
            return user;
        }

        /// <summary>
        /// Tạo người dùng mới (có thể truyền RoleName để tự tạo role).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<User>> Create(User user)
        {
            if (user.RoleId == 0 && !string.IsNullOrWhiteSpace(user.RoleName))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == user.RoleName);
                if (role == null)
                {
                    role = new Role { RoleName = user.RoleName, Description = "Auto-created" };
                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                }
                user.RoleId = role.Id;
            }

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }

        /// <summary>
        /// Cập nhật người dùng.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, User user)
        {
            if (id != user.Id) return BadRequest();

            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Cập nhật hồ sơ người dùng (username/email/role).
        /// </summary>
        [HttpPut("{id:int}/profile")]
        public async Task<IActionResult> UpdateProfile(int id, UpdateUserProfileRequest request)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Username = request.Username;
            user.Email = request.Email;
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                var role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == request.Role);
                if (role == null)
                {
                    role = new Role { RoleName = request.Role, Description = "Auto-created" };
                    _context.Roles.Add(role);
                    await _context.SaveChangesAsync();
                }
                user.RoleId = role.Id;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Khóa tài khoản người dùng.
        /// </summary>
        [HttpPost("{id:int}/lock")]
        public async Task<IActionResult> Lock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Mở khóa tài khoản người dùng.
        /// </summary>
        [HttpPost("{id:int}/unlock")]
        public async Task<IActionResult> Unlock(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.IsActive = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Reset mật khẩu người dùng.
        /// </summary>
        [HttpPost("{id:int}/reset-password")]
        public async Task<IActionResult> ResetPassword(int id, ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Mật khẩu mới không hợp lệ.");
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Password = request.NewPassword;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Xóa người dùng.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class UpdateUserProfileRequest
        {
            public string Username { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string? Role { get; set; }
        }

        public class ResetPasswordRequest
        {
            public string NewPassword { get; set; } = string.Empty;
        }
    }
}
