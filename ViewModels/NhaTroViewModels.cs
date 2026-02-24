using System;
using System.ComponentModel.DataAnnotations;

namespace do_an_tot_nghiep.ViewModels
{
    public class NhaTroListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tên Tòa Nhà")]
        public string TenNhaTro { get; set; } = string.Empty;

        [Display(Name = "Địa Chỉ")]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        [Display(Name = "Loại Nhà")]
        public int LoaiNha { get; set; }

        [Display(Name = "Giá Thuê Đầu Vào")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GiaThue { get; set; }

        [Display(Name = "Ngày Vận Hành")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? OperationDate { get; set; }
    }

    public class NhaTroFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên nhà")]
        [Display(Name = "Tên Tòa Nhà")]
        public string TenNhaTro { get; set; } = string.Empty;

        [Display(Name = "Phân Loại Nhà")]
        public int LoaiNha { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá thuê định mức")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá thuê phải >= 0")]
        [Display(Name = "Giá Thuê Định Mức")]
        public decimal GiaThue { get; set; }

        [Display(Name = "Ngày Thu Tiền Định Kỳ")]
        [Range(1, 31, ErrorMessage = "Ngày thu tiền từ 1 đến 31")]
        public int NgayThuTien { get; set; } = 1;

        [Display(Name = "Tỉnh/Thành phố")]
        public string? TinhThanh { get; set; }

        [Display(Name = "Quận/Huyện")]
        public string? QuanHuyen { get; set; }

        [Display(Name = "Phường/Xã")]
        public string? PhuongXa { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        [Display(Name = "Địa Chỉ Chi Tiết (Tên Đường, Số Nhà)")]
        public string DiaChiChiTiet { get; set; } = string.Empty;

        [Display(Name = "Danh sách tiện ích chung")]
        public string? DanhSachDichVu { get; set; }

        [Display(Name = "Ghi Chú Operations")]
        public string? GhiChu { get; set; }

        [Display(Name = "Ngày Bắt Đầu Vận Hành")]
        [DataType(DataType.Date)]
        public DateTime? OperationDate { get; set; }
    }
}
