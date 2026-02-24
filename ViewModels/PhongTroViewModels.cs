using System;
using System.ComponentModel.DataAnnotations;

namespace do_an_tot_nghiep.ViewModels
{
    public class PhongTroListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Tòa Nhà (Building)")]
        public int NhaTroId { get; set; }

        [Display(Name = "Mã Phòng (Code)")]
        public string TenPhong { get; set; } = string.Empty;

        [Display(Name = "Tầng")]
        public int Tang { get; set; }

        [Display(Name = "Diện Tích (m2)")]
        [DisplayFormat(DataFormatString = "{0:N2} m²")]
        public decimal DienTich { get; set; }

        [Display(Name = "Giá Phòng Chuẩn (Base)")]
        [DisplayFormat(DataFormatString = "{0:N0} đ")]
        public decimal GiaPhong { get; set; }

        [Display(Name = "Max Người")]
        public int SoLuongNguoi { get; set; }

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "Vacant";
    }

    public class PhongTroFormViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Mã/Tên phòng không được trống")]
        [Display(Name = "Mã Phòng / Tên Phòng")]
        public string TenPhong { get; set; } = string.Empty;

        [Required(ErrorMessage = "Bắt buộc chọn tòa nhà/chung cư")]
        [Display(Name = "Thuộc Tòa Nhà (BuildingID)")]
        public int NhaTroId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tầng")]
        [Display(Name = "Số Tầng")]
        public int Tang { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập giá phòng cơ sở")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phòng phải lớn hơn 0")]
        [Display(Name = "Giá Thuê Cơ Bản (Base Price)")]
        public decimal GiaPhong { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập diện tích")]
        [Display(Name = "Diện Tích (m2)")]
        public decimal DienTich { get; set; }

        [Required(ErrorMessage = "Số người tối đa 1-10")]
        [Range(1, 10, ErrorMessage = "Tối đa từ 1 đến 10 người / phòng")]
        [Display(Name = "Số Lượng Người Tối Đa")]
        public int SoLuongNguoi { get; set; } = 1;

        [Display(Name = "Loại Phòng")]
        public string? DoiTuong { get; set; } = "Standard";

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = "Vacant";

        // Mấy cái ở DB bạn [NotMapped] -> để UI xài tạm hoặc mốt tính sau
        [Display(Name = "Tiền Cọc (Gợi Ý)")]
        public decimal TienCoc { get; set; }

        [Display(Name = "Nội Thất Đặc Biệt")]
        public string? NoiThat { get; set; }

        [Display(Name = "Ghi Chú Vận Hành")]
        public string? GhiChu { get; set; }
    }
}
