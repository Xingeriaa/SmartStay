using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Tenants")]
    public class KhachThue
    {
        [Key]
        [Column("TenantId")]
        public int Id { get; set; }

        public int? UserId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [Column("FullName")]
        public string HoTen { get; set; } = string.Empty;

        [Column("PermanentAddress")]
        public string DiaChiThuongTru { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại chính")]
        [Column("Phone")]
        public string SoDienThoai1 { get; set; } = string.Empty;

        [NotMapped]
        public string? SoDienThoai2 { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
        [Column("CCCD")]
        public string SoCCCD { get; set; } = string.Empty;

        [NotMapped]
        public string? NoiCap { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        [Column("DateOfBirth")]
        public DateTime NgaySinh { get; set; }

        [NotMapped]
        public string? NoiSinh { get; set; }

        [NotMapped]
        public decimal? SoTienCoc { get; set; }

        [NotMapped]
        public string? HinhAnh { get; set; }

        public string? Gender { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<HopDongKhachThue> HopDongKhachThues { get; set; } = new List<HopDongKhachThue>();
    }
}
