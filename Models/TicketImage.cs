using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("TicketImages")]
    public class TicketImage
    {
        [Key]
        [Column("ImageId")]
        public long Id { get; set; }

        public long TicketId { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
