using System;
using System.ComponentModel.DataAnnotations;

namespace do_an_tot_nghiep.ViewModels
{
    public class ContractExtensionViewModel
    {
        public int ContractId { get; set; }

        [Display(Name = "Mã Hợp Đồng")]
        public string ContractCode { get; set; } = string.Empty;

        [Display(Name = "Ngày Kết Thúc (Hiện Tại)")]
        [DataType(DataType.Date)]
        public DateTime OldEndDate { get; set; }

        [Display(Name = "Giá Thuê Gốc")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal OldRentPrice { get; set; }

        [Required(ErrorMessage = "Bắt buộc nhập ngày gia hạn kết thúc mới")]
        [Display(Name = "Gia Hạn Tới Ngày")]
        [DataType(DataType.Date)]
        public DateTime NewEndDate { get; set; }

        [Display(Name = "Giá Thuê Mới (Nếu Có)")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal NewRentPrice { get; set; }
    }
}
