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
        public long Id { get; set; }

        public int RoomId { get; set; }

        [Required]
        public string MonthYear { get; set; } = string.Empty;

        [Column("OldElectricityIndex")]
        public int OldElectricityIndex { get; set; }

        [Column("NewElectricityIndex")]
        public int NewElectricityIndex { get; set; }

        [Column("OldWaterIndex")]
        public int OldWaterIndex { get; set; }

        [Column("NewWaterIndex")]
        public int NewWaterIndex { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? ConsumptionElectricity { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public int? ConsumptionWater { get; set; }

        public long? PreviousReadingId { get; set; }

        public DateTime ReadingDate { get; set; }

        [NotMapped]
        public int ElectricityIndex
        {
            get => NewElectricityIndex;
            set => NewElectricityIndex = value;
        }

        [NotMapped]
        public int WaterIndex
        {
            get => NewWaterIndex;
            set => NewWaterIndex = value;
        }

        [NotMapped]
        public string? ImageProof { get; set; }

        public int? RecordedBy { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
