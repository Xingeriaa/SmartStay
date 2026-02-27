using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("AuditLogs")]
    public class AuditLog
    {
        [Key]
        [Column("AuditId")]
        public long Id { get; set; }

        public int? UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string EntityName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? EntityId { get; set; }

        public string? OldValue { get; set; }
        public string? NewValue { get; set; }

        [StringLength(45)]
        public string? IPAddress { get; set; }
        public string? UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
