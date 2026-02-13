using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Invoices")]
    public class Invoice
    {
        [Key]
        [Column("InvoiceId")]
        public int Id { get; set; }

        [Required]
        public string InvoiceCode { get; set; } = string.Empty;

        public int ContractId { get; set; }

        [Required]
        public string? Period { get; set; }

        [Required]
        public string? MonthYear { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal SubTotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TaxAmount { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime? DueDate { get; set; }
        public DateTime? PaymentDate { get; set; }

        public string Status { get; set; } = "Unpaid";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
    }
}
