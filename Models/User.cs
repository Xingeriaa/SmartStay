using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        [Column("UserId")]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [Column("PasswordHash")]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }
        public string? Phone { get; set; }

        public int RoleId { get; set; }
        public Role? Role { get; set; }

        public string? RefreshToken { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public string? RoleName
        {
            get => Role?.RoleName;
            set
            {
                // Binding helper; controller should map RoleName -> RoleId
            }
        }

        [NotMapped]
        public bool IsLocked
        {
            get => !IsActive;
            set => IsActive = !value;
        }
    }
}
