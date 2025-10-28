using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WBH.Models;

namespace WBH.Models
{
    [Serializable] // để lưu được trong Session
    public class CartItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}