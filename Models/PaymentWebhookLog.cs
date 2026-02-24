using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("PaymentWebhookLogs")]
    public class PaymentWebhookLog
    {
        [Key]
        [Column("LogId")]
        public long Id { get; set; }

        [Required]
        public string Provider { get; set; } = string.Empty;

        public string PayloadJson { get; set; } = string.Empty;

        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        public string Status { get; set; } = "Pending";

        public int RetryCount { get; set; } = 0;

        public string? ErrorMessage { get; set; }
    }
}
