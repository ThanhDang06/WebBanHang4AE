using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WBH.Models;

namespace WBH.Helpers
{
    public static class SortHelper
    {
        public static IQueryable<Product> ApplySort(IQueryable<Product> query, string sortOrder)
        {
            switch (sortOrder)
            {
                case "price_asc": return query.OrderBy(p => p.Price);
                case "price_desc": return query.OrderByDescending(p => p.Price);
                case "name_asc": return query.OrderBy(p => p.ProductName);
                case "name_desc": return query.OrderByDescending(p => p.ProductName);
                default: return query.OrderBy(p => p.IDProduct);
            }
        }
    }

}