using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("RoomAnalyticsSnapshots")]
    public class RoomAnalyticsSnapshot
    {
        [Key]
        public int Id { get; set; }

        public int BuildingId { get; set; }
        
        [ForeignKey("BuildingId")]
        public NhaTro Building { get; set; } = null!;

        public int SnapshotYear { get; set; }
        public int SnapshotMonth { get; set; }

        public int TotalRooms { get; set; }
        public int OccupiedRooms { get; set; }
        public int VacantRooms { get; set; }
        public int MaintenanceRooms { get; set; }

        [Column(TypeName = "decimal(5,2)")]
        public decimal OccupancyRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalMaintenanceCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal ExpectedRevenue { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal ActualRevenue { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
