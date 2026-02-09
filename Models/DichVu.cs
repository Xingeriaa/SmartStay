using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Services")]
    public class DichVu
    {
        [Key]
        [Column("ServiceId")]
        public int Id { get; set; }

        [Required]
        [Column("ServiceName")]
        public string Ten { get; set; } = string.Empty;

        [Required]
        [Column("ChargeType")]
        public string LoaiDichVu { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public decimal DonGia { get; set; }

        [NotMapped]
        public string? GhiChu { get; set; }

        public ICollection<ServicePriceHistory> PriceHistory { get; set; } = new List<ServicePriceHistory>();
    }
}
