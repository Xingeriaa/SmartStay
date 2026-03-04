using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("MonthlyServiceUsages")]
    public class MonthlyServiceUsage
    {
        [Key]
        public long Id { get; set; }

        public int ContractId { get; set; }
        public int ServiceId { get; set; }

        public DateTime MonthYear { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; }

        [MaxLength(200)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ContractId")]
        public HopDong? Contract { get; set; }

        [ForeignKey("ServiceId")]
        public DichVu? Service { get; set; }
    }
}
