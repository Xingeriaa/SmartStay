using do_an_tot_nghiep.Models;
using Microsoft.EntityFrameworkCore;

namespace do_an_tot_nghiep.Services
{
    public interface IAccountService
    {
        /// <summary>
        /// Đăng ký tự do — luôn gán role "Tenant".
        /// Role KHÔNG được nhận từ client.
        /// </summary>
        Task<RegisterResult> RegisterAsync(string username, string password, string email,
                                           string? fullName = null, string? phone = null);

        Task<LoginResult> LoginAsync(string username, string password);
    }

    public sealed record RegisterResult(bool Success, string? ErrorMessage);
    public sealed record LoginResult(bool Success, string? ErrorMessage, User? User, string? RoleName);

    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;

        // Khớp chính xác với DB: Roles.RoleName = "Tenant"
        private const string PUBLIC_ROLE = "Tenant";

        public AccountService(ApplicationDbContext context)
        {
            _context = context;
        }

        // ═══════════════════════════════════════════════════════════════
        // ĐĂNG KÝ
        // ═══════════════════════════════════════════════════════════════
        public async Task<RegisterResult> RegisterAsync(
            string username, string password, string email,
            string? fullName = null, string? phone = null)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 4)
                return new RegisterResult(false, "Tên đăng nhập phải có ít nhất 4 ký tự.");
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return new RegisterResult(false, "Mật khẩu phải có ít nhất 8 ký tự.");
            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
                return new RegisterResult(false, "Email không hợp lệ.");

            if (await _context.Users.AnyAsync(u => u.Username == username && !u.IsDeleted))
                return new RegisterResult(false, "Tên đăng nhập đã tồn tại.");
            if (await _context.Users.AnyAsync(u => u.Email == email && !u.IsDeleted))
                return new RegisterResult(false, "Email đã được sử dụng bởi tài khoản khác.");

            // Lấy role "Tenant" — tự seed nếu chưa có (an toàn: cố định PUBLIC_ROLE)
            var roleEntity = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == PUBLIC_ROLE);

            if (roleEntity == null)
            {
                roleEntity = new Role
                {
                    RoleName = PUBLIC_ROLE,
                    Description = "Người thuê phòng — quyền tự phục vụ"
                };
                _context.Roles.Add(roleEntity);
                await _context.SaveChangesAsync();
            }

            var newUser = new User
            {
                Username = username.Trim(),
                Password = password,  // TODO production: BCrypt.HashPassword(password)
                Email = email.Trim().ToLowerInvariant(),
                FullName = string.IsNullOrWhiteSpace(fullName) ? username : fullName.Trim(),
                Phone = phone?.Trim(),
                RoleId = roleEntity.Id,
                IsActive = true,
                IsDeleted = false
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new RegisterResult(true, null);
        }

        // ═══════════════════════════════════════════════════════════════
        // ĐĂNG NHẬP
        // ═══════════════════════════════════════════════════════════════
        public async Task<LoginResult> LoginAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return new LoginResult(false, "Vui lòng nhập đầy đủ thông tin.", null, null);

            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u =>
                    u.Username == username &&
                    u.Password == password &&  // TODO: BCrypt.Verify(password, u.Password)
                    !u.IsDeleted);

            if (user == null)
                return new LoginResult(false, "Sai tên đăng nhập hoặc mật khẩu.", null, null);

            if (!user.IsActive)
                return new LoginResult(false, "Tài khoản đã bị khóa. Liên hệ quản trị viên.", null, null);

            // Normalize tên role cũ → chuẩn DB mới ("Admin" / "Staff" / "Tenant")
            var normalizedRole = NormalizeRole(user.Role?.RoleName ?? "");

            return new LoginResult(true, null, user, normalizedRole);
        }

        // ═══════════════════════════════════════════════════════════════
        // Normalize role name — xử lý dữ liệu legacy trong DB
        // ═══════════════════════════════════════════════════════════════
        private static string NormalizeRole(string raw) => raw.Trim().ToLowerInvariant() switch
        {
            // ── Admin ──
            "admin" => "Admin",
            "chutro" => "Admin",    // legacy VN
            "chu tro" => "Admin",
            "quantri" => "Admin",
            "quanly" => "Admin",
            "owner" => "Admin",

            // ── Staff ──
            "staff" => "Staff",
            "nhanvien" => "Staff",    // legacy VN
            "nhan vien" => "Staff",
            "manager" => "Staff",

            // ── Tenant ──
            "tenant" => "Tenant",
            "nguoithue" => "Tenant",   // legacy VN
            "nguoi thue" => "Tenant",
            "khachhang" => "Tenant",
            "khach hang" => "Tenant",
            "customer" => "Tenant",

            // Giữ nguyên nếu không nhận ra (capitalize chữ đầu)
            _ => raw.Length > 0 ? char.ToUpper(raw[0]) + raw[1..] : raw
        };
    }
}
