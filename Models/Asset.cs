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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
