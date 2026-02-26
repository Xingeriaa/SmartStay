using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace do_an_tot_nghiep.Models
{
    [Table("Rooms")]
    public class PhongTro
    {
        [Key]
        [Column("RoomId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phòng")]
        [Column("RoomCode")]
        public string TenPhong { get; set; } = string.Empty;

        [Column("BaseRentPrice", TypeName = "decimal(18,2)")]
        public decimal GiaPhong { get; set; }

        [Column("FloorNumber")]
        public int Tang { get; set; }

        [Column("Area", TypeName = "decimal(10,2)")]
        public decimal DienTich { get; set; }

        [Range(1, 2, ErrorMessage = "Chỉ cho phép tối đa 2 người/phòng")]
        [Column("MaxOccupants")]
        public int SoLuongNguoi { get; set; }

        [NotMapped]
        public decimal TienCoc { get; set; }

        [Column("RoomType")]
        public string? DoiTuong { get; set; }     // Nam / Nu / Trong

        [NotMapped]
        public string? DichVu { get; set; }

        [NotMapped]
        public string? AnhPhong { get; set; }

        [NotMapped]
        public string? NoiThat { get; set; }

        [NotMapped]
        public string? GhiChu { get; set; }

        [Column("BuildingId")]
        public int NhaTroId { get; set; }

        [ForeignKey(nameof(NhaTroId))]
        public NhaTro? NhaTro { get; set; }

        public ICollection<HopDong> HopDongs { get; set; } = new List<HopDong>();

        public byte Status { get; set; } = 1; // 1 = Vacant, 2 = Occupied, 3 = Maintenance
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- Cột nâng cấp SAFE ---
        public bool Balcony { get; set; } = false;
        public bool HasPrivateBathroom { get; set; } = false;
        public string? Orientation { get; set; }
        public int? ConditionScore { get; set; }
        public int? NoiseLevelRating { get; set; }
        public DateTime? LastInspectionDate { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
