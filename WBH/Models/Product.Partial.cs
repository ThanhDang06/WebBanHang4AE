using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    public partial class Product
    {
        public bool IsOutOfStock
        {
            get
            {
                // Dùng Quantity trong DB để xác định
                return !this.Quantity.HasValue || this.Quantity.Value <= 0;
            }
        }
    }
}