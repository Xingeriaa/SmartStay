using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Assets")]
    public class Asset
    {
        [Key]
        [Column("AssetId")]
        public int Id { get; set; }

        [Required]
        public string AssetName { get; set; } = string.Empty;

        public string? Brand { get; set; }
        public string? Model { get; set; }
        public int? WarrantyMonths { get; set; }
        public string? Description { get; set; }

        // --- Cột nâng cấp SAFE ---
        public string? Category { get; set; }
        public int? DepreciationYears { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? UnitPrice { get; set; }
        public string? Supplier { get; set; }
        public string? QRCode { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
