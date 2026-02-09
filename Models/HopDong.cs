using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Contracts")]
    public class HopDong
    {
        [Key]
        [Column("ContractId")]
        public int Id { get; set; }

        [Required]
        public string ContractCode { get; set; } = string.Empty;

        [Required]
        [Column("RoomId")]
        public int PhongTroId { get; set; }

        [ForeignKey(nameof(PhongTroId))]
        public PhongTro PhongTro { get; set; } = null!;

        [Required]
        [Column("StartDate")]
        public DateTime NgayBatDau { get; set; }

        [Required]
        [Column("EndDate")]
        public DateTime? NgayKetThuc { get; set; }

        [Required]
        public DateTime SignatureDate { get; set; } = DateTime.Today;

        public int PaymentCycle { get; set; } = 1;

        [Required]
        [Column("RentPriceSnapshot", TypeName = "decimal(18,2)")]
        public decimal GiaThue { get; set; }

        [Required]
        [Column("DepositAmount", TypeName = "decimal(18,2)")]
        public decimal TienCoc { get; set; }

        public string DepositStatus { get; set; } = "Available";

        [Required]
        public TrangThaiHopDong TrangThai { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [NotMapped]
        public DateTime? NgayThanhLy { get; set; }

        [NotMapped]
        public decimal? TongTienQuyetToan { get; set; }

        [NotMapped]
        public decimal? SoTienHoanTra { get; set; }

        [NotMapped]
        public string? GhiChuThanhLy { get; set; }

        public Liquidation? Liquidation { get; set; }

        public ICollection<HopDongKhachThue> HopDongKhachThues { get; set; } = new List<HopDongKhachThue>();
    }
}
