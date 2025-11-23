using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WBH.Models
{
    public class AddToCartModel
    {
            public int id { get; set; }
            public int? colorId { get; set; }
            public int? sizeId { get; set; }
            public int quantity { get; set; }       
    }
}