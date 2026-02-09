using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("MeterReadings")]
    public class MeterReading
    {
        [Key]
        [Column("ReadingId")]
        public int Id { get; set; }

        public int RoomId { get; set; }

        [Required]
        public string MonthYear { get; set; } = string.Empty;

        public DateTime ReadingDate { get; set; }

        public int ElectricityIndex { get; set; }
        public int WaterIndex { get; set; }

        public string? ImageProof { get; set; }

        public int RecordedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
