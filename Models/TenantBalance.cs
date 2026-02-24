using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("TenantBalances")]
    public class TenantBalance
    {
        [Key]
        [Column("BalanceId")]
        public int Id { get; set; }

        public int TenantId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAmount { get; set; }

        public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TenantId))]
        public KhachThue? KhachThue { get; set; }
    }
}
