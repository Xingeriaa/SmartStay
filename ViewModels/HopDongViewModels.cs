using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace do_an_tot_nghiep.ViewModels
{
    public class HopDongListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã Hợp Đồng")]
        public string ContractCode { get; set; } = string.Empty;

        public int PhongTroId { get; set; }

        [Display(Name = "Tên Phòng")]
        public string? TenPhong { get; set; }

        [Display(Name = "Ngày Bắt Đầu")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime NgayBatDau { get; set; }

        [Display(Name = "Ngày Kết Thúc (Dự kiến)")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime? NgayKetThuc { get; set; }

        [Display(Name = "Trạng Thái")]
        public string TrangThai { get; set; } = string.Empty;

        [Display(Name = "Tiền Cọc")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal TienCoc { get; set; }

        [Display(Name = "Giá Thuê (Snapshot)")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GiaThue { get; set; }
    }

    public class HopDongFormViewModel
    {
        public int Id { get; set; } // If id > 0 it's update/details mode

        [Required(ErrorMessage = "Bắt buộc chọn phòng để lập hợp đồng")]
        [Display(Name = "Phòng (Room)")]
        public int PhongTroId { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu tính tiền thuê là bắt buộc")]
        [Display(Name = "Ngày Bắt Đầu Thuê")]
        [DataType(DataType.Date)]
        public DateTime NgayBatDau { get; set; } = DateTime.Today;

        [Display(Name = "Ngày Kết Thúc (Nếu Có)")]
        [DataType(DataType.Date)]
        public DateTime? NgayKetThuc { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Chu Kỳ Thanh Toán (tháng)")]
        [Display(Name = "Chu Kỳ Đóng Tiền (Tháng)")]
        [Range(1, 12, ErrorMessage = "Chu kỳ đóng tiền phải từ 1 đến 12 tháng")]
        public int PaymentCycle { get; set; } = 1;

        [Required(ErrorMessage = "Tiền cọc không được để trống")]
        [Display(Name = "Tiền Cọc Tạm Giữ")]
        [Range(0, double.MaxValue, ErrorMessage = "Tiền cọc định mức không được âm")]
        public decimal TienCoc { get; set; }

        [Required(ErrorMessage = "Cần ít nhất một người đại diện/cư dân để lập hợp đồng")]
        [Display(Name = "Danh Sách Cư Dân Vào Ở")]
        public List<int> KhachThueIds { get; set; } = new List<int>();

        // Select lists needed for the View drop-downs
        public IEnumerable<SelectListItem>? AvailableRooms { get; set; }
        public IEnumerable<SelectListItem>? AvailableTenants { get; set; }
        public List<ServiceSelectionViewModel> Services { get; set; } = new List<ServiceSelectionViewModel>();
    }

    public class ServiceSelectionViewModel
    {
        public int ServiceId { get; set; }
        public string? ServiceName { get; set; }
        public string? ChargeType { get; set; }
        public decimal UnitPrice { get; set; }
        public bool IsSelected { get; set; }
        public int Quantity { get; set; }
    }
}
