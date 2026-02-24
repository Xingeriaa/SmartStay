using System;
using System.ComponentModel.DataAnnotations;

namespace do_an_tot_nghiep.ViewModels
{
    public class UserListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tên Đăng Nhập")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "Quyền Truy Cập")]
        public string RoleName { get; set; } = string.Empty;

        [Display(Name = "Trạng Thái")]
        public bool IsActive { get; set; }

        [Display(Name = "Lần Cuối Đăng Nhập")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime? LastLoginAt { get; set; }
    }

    public class UserFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc")]
        [Display(Name = "Tên Đăng Nhập (Username)")]
        public string Username { get; set; } = string.Empty;

        [Display(Name = "Mật Khẩu Mới (Bỏ trống nếu không đổi)")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Trường này là bắt buộc")]
        [Display(Name = "Họ và Tên")]
        public string FullName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Địa chỉ Email")]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "SĐT không hợp lệ")]
        [Display(Name = "Số Điện Thoại")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Phân quyền không được để trống")]
        [Display(Name = "Bậc Phân Quyền (Role)")]
        public string RoleName { get; set; } = "Staff"; // Default or Dropdown binding

        [Display(Name = "Trạng Thái Hoạt Động")]
        public bool IsActive { get; set; } = true;
    }
}
