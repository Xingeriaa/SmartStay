using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("TenantBalanceTransactions")]
    public class TenantBalanceTransaction
    {
        [Key]
        [Column("TransactionId")]
        public long Id { get; set; }

        public int TenantId { get; set; }
        public int BalanceId { get; set; }

        [Required]
        public string TransactionType { get; set; } = string.Empty; // Overpayment, Refund, ManualTopUp, ManualDeduct, AutoOffset

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public int? RelatedInvoiceId { get; set; }
        public long? RelatedPaymentId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBefore { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; }

        [MaxLength(255)]
        public string? Note { get; set; }

        [MaxLength(255)]
        public string? Reason { get; set; }

        public int CreatedBy { get; set; } // FK to Users

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(TenantId))]
        public KhachThue? KhachThue { get; set; }

        [ForeignKey(nameof(BalanceId))]
        public TenantBalance? Balance { get; set; }

        [ForeignKey(nameof(RelatedInvoiceId))]
        public Invoice? RelatedInvoice { get; set; }

        [ForeignKey(nameof(RelatedPaymentId))]
        public PaymentTransaction? RelatedPayment { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        public User? User { get; set; }
    }
}
