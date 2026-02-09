using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("UserPermissions")]
    public class UserPermission
    {
        [Key]
        [Column("PermissionId")]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        public string PermissionKey { get; set; } = string.Empty;

        public int? BuildingId { get; set; }
        public int? RoomId { get; set; }

        public bool IsAllowed { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
