using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("Tickets")]
    public class Ticket
    {
        [Key]
        [Column("TicketId")]
        public int Id { get; set; }

        public int CreatedBy { get; set; }
        public int? RoomId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string Priority { get; set; } = "Medium";

        public string Status { get; set; } = "Open";

        public int? AssignedTo { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<TicketImage> Images { get; set; } = new List<TicketImage>();
    }
}
