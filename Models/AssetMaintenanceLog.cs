using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("AssetMaintenanceLogs")]
    public class AssetMaintenanceLog
    {
        [Key]
        public int MaintenanceId { get; set; }

        public int RoomAssetId { get; set; }

        [ForeignKey("RoomAssetId")]
        public RoomAsset? RoomAsset { get; set; }

        [Required]
        public DateTime MaintenanceDate { get; set; }

        [Required]
        public string IssueDescription { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }

        [MaxLength(200)]
        public string? PerformedBy { get; set; }

        public DateTime? NextMaintenanceDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
