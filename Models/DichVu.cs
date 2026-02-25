using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace do_an_tot_nghiep.Models
{
    [Table("Services")]
    public class DichVu
    {
        [Key]
        [Column("ServiceId")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên dịch vụ")]
        [Column("ServiceName")]
        public string Ten { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn loại dịch vụ")]
        [Column("ChargeType")]
        public string LoaiDichVu { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        [Required(ErrorMessage = "Vui lòng nhập đơn giá")]
        public decimal DonGia { get; set; }

        [NotMapped]
        public string? GhiChu { get; set; }

        [ValidateNever]
        public ICollection<ServicePriceHistory> PriceHistory { get; set; } = new List<ServicePriceHistory>();
    }
}
