using System;
using System.ComponentModel.DataAnnotations;

namespace do_an_tot_nghiep.ViewModels
{
    public class KhachThueListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Họ Tên")]
        public string HoTen { get; set; } = string.Empty;

        [Display(Name = "Số CCCD")]
        public string SoCCCD { get; set; } = string.Empty;

        [Display(Name = "Điện Thoại")]
        public string SoDienThoai1 { get; set; } = string.Empty;

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "Active";

        [Display(Name = "Ngày Sinh")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NgaySinh { get; set; }
    }

    public class KhachThueFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập họ tên khách thuê")]
        [Display(Name = "Họ và Tên")]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số CCCD")]
        [Display(Name = "Số Căn Cước (CCCD)")]
        [StringLength(12, MinimumLength = 9, ErrorMessage = "CCCD phải từ 9-12 số")]
        public string SoCCCD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại liên lạc")]
        [Phone(ErrorMessage = "Sai định dạng số điện thoại")]
        [Display(Name = "Số điện thoại chính")]
        public string SoDienThoai1 { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại phụ (Zalo/Người thân)")]
        [Phone(ErrorMessage = "Sai định dạng số điện thoại")]
        public string? SoDienThoai2 { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Địa chỉ Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập ngày sinh")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày Sinh")]
        public DateTime NgaySinh { get; set; }

        [Display(Name = "Giới Tính")]
        public string? Gender { get; set; }

        [Display(Name = "Nơi Sinh")]
        public string? NoiSinh { get; set; }

        [Display(Name = "Nơi Cấp CCCD")]
        public string? NoiCap { get; set; }

        [Display(Name = "Địa Chỉ Thường Trú")]
        public string DiaChiThuongTru { get; set; } = string.Empty;

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "Active";

        [Display(Name = "Hình Ảnh Đại Diện")]
        public string? HinhAnh { get; set; }
    }
}
