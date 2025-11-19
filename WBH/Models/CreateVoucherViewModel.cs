using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    public class CreateVoucherViewModel
    {
        [Required]
        public string Type { get; set; } // PERCENT hoặc AMOUNT

        [Required]
        [Range(0, 1000000)]
        public decimal Value { get; set; } // Giá trị giảm

        [Required]
        [Range(0, 1000000)]
        public decimal MinOrderAmount { get; set; } // Đơn tối thiểu

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime StartDate { get; set; } // Ngày bắt đầu

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime EndDate { get; set; } // Ngày kết thúc

        [Required]
        [Range(1, 1000)]
        public int RemainingUses { get; set; } // Số lượt dùng

        public int? IDCus { get; set; } 
    }
}