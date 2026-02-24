using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace do_an_tot_nghiep.ViewModels
{
    public class InvoiceListItemViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã Hóa Đơn")]
        public string InvoiceCode { get; set; } = string.Empty;

        public int ContractId { get; set; }

        [Display(Name = "Tháng")]
        public string MonthYear { get; set; } = string.Empty;

        [Display(Name = "Tổng Tiền")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Trạng Thái")]
        public string Status { get; set; } = string.Empty;

        [Display(Name = "Hạn Thanh Toán")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime DueDate { get; set; }

        [Display(Name = "Tên Phòng")]
        public string RoomName { get; set; } = "N/A";
    }

    public class InvoiceDetailViewModel
    {
        public int Id { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public string MonthYear { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public string RoomName { get; set; } = "N/A";
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }

        public List<InvoiceLineItem> Lines { get; set; } = new List<InvoiceLineItem>();
    }

    public class InvoiceLineItem
    {
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
    }

    public class GenerateInvoiceViewModel
    {
        [Required(ErrorMessage = "Bắt buộc chọn Hợp Đồng")]
        [Display(Name = "Hợp Đồng (Phòng)")]
        public int ContractId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Tháng/Năm")]
        [Display(Name = "Kỳ Hóa Đơn (YYYY-MM)")]
        public string MonthYear { get; set; } = DateTime.Today.ToString("yyyy-MM");

        [Display(Name = "Hạn Thanh Toán")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(5);

        public IEnumerable<SelectListItem>? AvailableContracts { get; set; }
    }
}
