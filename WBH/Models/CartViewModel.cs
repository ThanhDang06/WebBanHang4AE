using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    [Serializable]
    public class CartViewModel
    {
        public List<Cart> Items { get; set; } = new List<Cart>(); // danh sách sản phẩm trong giỏ
        public string VoucherCode { get; set; } // mã voucher khách nhập
        public decimal Discount { get; set; }   // tiền giảm
        public decimal TotalAmount => Items.Sum(i => (i.Product.Price ?? 0) * i.Quantity);
        public decimal FinalAmount => TotalAmount - Discount;
    }
}