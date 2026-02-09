using do_an_tot_nghiep.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IAccountService
    {
        Task<RegisterResult> RegisterAsync(string username, string password, string email, string role);
        Task<LoginResult> LoginAsync(string username, string password);
    }

    public sealed record RegisterResult(bool Success, string? ErrorMessage);

    public sealed record LoginResult(bool Success, string? ErrorMessage, User? User, string? RoleName);

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<RegisterResult> RegisterAsync(string username, string password, string email, string role)
        {
            if (await _context.Users.AnyAsync(u => u.Username == username))
            {
                return new RegisterResult(false, "Tên đăng nhập đã tồn tại!");
            }

            var roleEntity = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == role);
            if (roleEntity == null)
            {
                roleEntity = new Role
                {
                    RoleName = role,
                    Description = "Auto-created"
                };
                _context.Roles.Add(roleEntity);
                await _context.SaveChangesAsync();
            }

            var newUser = new User
            {
                Username = username,
                Password = password,
                Email = email,
                FullName = username,
                RoleId = roleEntity.Id,
                IsActive = true,
                IsDeleted = false
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new RegisterResult(true, null);
        }

        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password && !u.IsDeleted);

            if (user == null)
            {
                return new LoginResult(false, "Sai tài khoản hoặc mật khẩu!", null, null);
            }

            if (!user.IsActive)
            {
                return new LoginResult(false, "Tài khoản đã bị khóa!", null, null);
            }

            return new LoginResult(true, null, user, user.Role?.RoleName);
        }
    }
}
