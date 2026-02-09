using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Liquidations")]
    public class Liquidation
    {
        [Key]
        [Column("LiquidationId")]
        public int Id { get; set; }

        public int ContractId { get; set; }
        public HopDong? Contract { get; set; }

        public string? Reason { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal FinalInvoiceAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DepositUsed { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal RefundAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal AdditionalCharge { get; set; }

        public int StaffId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
