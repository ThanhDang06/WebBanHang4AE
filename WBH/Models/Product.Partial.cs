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
            get { return Quantity.HasValue && Quantity.Value <= 0; }
        }
    }
}