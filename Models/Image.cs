using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Images")]
    public class Image
    {
        [Key]
        [Column("ImageId")]
        public int Id { get; set; }

        public int? BuildingId { get; set; }
        public int? RoomId { get; set; }
        public int? AssetId { get; set; }

        [ForeignKey("BuildingId")]
        public NhaTro? Building { get; set; }

        [ForeignKey("RoomId")]
        public PhongTro? Room { get; set; }

        [ForeignKey("AssetId")]
        public Asset? Asset { get; set; }

        [Required]
        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsThumbnail { get; set; } = false;

        public int? UploadedBy { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
