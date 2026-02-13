using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace do_an_tot_nghiep.Models
{
    [Table("ContractServices")]
    public class ContractService
    {
        [Key]
        public long Id { get; set; }

        public int ContractId { get; set; }
        public int ServiceId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Quantity { get; set; } = 1;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPriceSnapshot { get; set; }
    }
}
