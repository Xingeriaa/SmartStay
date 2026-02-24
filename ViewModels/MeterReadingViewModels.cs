using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace do_an_tot_nghiep.ViewModels
{
    public class MeterReadingListItemViewModel
    {
        public long Id { get; set; }

        public int RoomId { get; set; }

        [Display(Name = "Tên Phòng")]
        public string RoomName { get; set; } = string.Empty;

        [Display(Name = "Kỳ Ghi (YYYY-MM)")]
        public string MonthYear { get; set; } = string.Empty;

        [Display(Name = "Điện Cũ")]
        public long OldElectricityIndex { get; set; }

        [Display(Name = "Điện Mới")]
        public long NewElectricityIndex { get; set; }

        [Display(Name = "Nước Cũ")]
        public long OldWaterIndex { get; set; }

        [Display(Name = "Nước Mới")]
        public long NewWaterIndex { get; set; }

        [Display(Name = "Ngày Ghi")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}")]
        public DateTime ReadingDate { get; set; }
    }

    public class MeterReadingFormViewModel
    {
        public long Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phòng")]
        [Display(Name = "Phòng Trọ")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Kỳ phát sinh bắt buộc")]
        [Display(Name = "Kỳ Chốt (YYYY-MM)")]
        public string MonthYear { get; set; } = DateTime.Today.ToString("yyyy-MM");

        [Required(ErrorMessage = "Thiếu chỉ số Điện mới")]
        [Display(Name = "Chỉ Số Điện Mới")]
        [Range(0, long.MaxValue, ErrorMessage = "Chỉ số điện không được Âm")]
        public long NewElectricityIndex { get; set; }

        [Display(Name = "Chỉ Số Điện Cũ (Optional - Tự Lấy)")]
        [Range(0, long.MaxValue, ErrorMessage = "Chỉ số không hợp lệ")]
        public long OldElectricityIndex { get; set; }

        [Required(ErrorMessage = "Thiếu chỉ số Nước mới")]
        [Display(Name = "Chỉ Số Nước Mới")]
        [Range(0, long.MaxValue, ErrorMessage = "Chỉ số nước không được Âm")]
        public long NewWaterIndex { get; set; }

        [Display(Name = "Chỉ Số Nước Cũ (Optional - Tự Lấy)")]
        [Range(0, long.MaxValue, ErrorMessage = "Chỉ số không hợp lệ")]
        public long OldWaterIndex { get; set; }

        [Display(Name = "Ghi Chú")]
        public string? Note { get; set; }

        public IEnumerable<SelectListItem>? AvailableRooms { get; set; }
    }
}
