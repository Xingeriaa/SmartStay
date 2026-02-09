using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("PaymentTransactions")]
    public class PaymentTransaction
    {
        [Key]
        [Column("TransactionId")]
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public string PaymentMethod { get; set; } = string.Empty;

        public string? BankName { get; set; }
        public string? TransactionCode { get; set; }
        public string? EvidenceImage { get; set; }

        public DateTime PaidAt { get; set; }

        public int? ConfirmedBy { get; set; }
    }
}
