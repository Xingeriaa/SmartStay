using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("ContractExtensions")]
    public class ContractExtension
    {
        [Key]
        [Column("ExtensionId")]
        public int Id { get; set; }

        public int ContractId { get; set; }

        public DateTime OldEndDate { get; set; }
        public DateTime NewEndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal NewRentPriceSnapshot { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
