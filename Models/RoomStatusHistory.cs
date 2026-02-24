using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("RoomStatusHistories")]
    public class RoomStatusHistory
    {
        [Key]
        [Column("HistoryId")]
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public PhongTro? PhongTro { get; set; }

        [Required]
        [MaxLength(50)]
        public string OldStatus { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NewStatus { get; set; } = string.Empty;

        public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(255)]
        public string? Reason { get; set; }

        public int? ChangedByUserId { get; set; }
    }
}
