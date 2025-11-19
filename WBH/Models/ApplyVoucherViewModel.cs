using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    public class ApplyVoucherViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã voucher")]
        [Display(Name = "Mã Voucher")]
        public string Code { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng đơn hàng không hợp lệ")]
        [Display(Name = "Tổng tiền đơn hàng")]
        public decimal OrderAmount { get; set; }
    }
}