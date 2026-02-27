using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace do_an_tot_nghiep.Models
{
    [Table("ServicePriceHistory")]
    public class ServicePriceHistory
    {
        [Key]
        [Column("PriceId")]
        public int Id { get; set; }

        public int ServiceId { get; set; }
        [JsonIgnore]
        [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
        public DichVu? Service { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
