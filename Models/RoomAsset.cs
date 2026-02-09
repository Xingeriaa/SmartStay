using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("RoomAssets")]
    public class RoomAsset
    {
        [Key]
        [Column("AssetId")]
        public int Id { get; set; }

        public int RoomId { get; set; }

        [Required]
        public string AssetName { get; set; } = string.Empty;

        public string? SerialNumber { get; set; }

        public int Quantity { get; set; } = 1;

        public int Status { get; set; } = 1;

        public DateTime? PurchaseDate { get; set; }
        public DateTime? WarrantyExpiry { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
