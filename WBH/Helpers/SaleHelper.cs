using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WBH.Models;

namespace WBH.Helpers
{
    public static class SaleHelper
    {    
        public static decimal GetSalePrice(Product product, decimal? discountPercent)
        {
            if (product == null)
                return 0m;

            // Ưu tiên dùng OldPrice nếu có, vì đó là giá gốc thật
            var basePrice = product.OldPrice ?? product.Price ?? 0m;

            // Nếu giá <= 0 thì không tính sale
            if (basePrice <= 0)
                return 1000m; // Giá tối thiểu an toàn

            var discount = (discountPercent ?? 0m) / 100m;

            var salePrice = basePrice * (1 - discount);
            // Đảm bảo giá sau giảm vẫn hợp lệ
            if (salePrice <= 0)
                salePrice = basePrice; // fallback về giá gốc

            return salePrice;
        }


        public static decimal GetFinalPrice(Product product, DBFashionStoreEntitiess db, DateTime now)
        {
            if (product == null)
                return 0m;

            var sale = db.Sales.FirstOrDefault(s => s.IDProduct == product.IDProduct
                                                   && (s.Active ?? false)
                                                   && s.StartDate <= now
                                                   && s.EndDate >= now);

            var basePrice = product.OldPrice ?? product.Price ?? 0m;

            if (sale == null || basePrice <= 0)
                return basePrice > 0 ? basePrice : 1000m;

            var discount = (sale.DiscountPercent ?? 0m) / 100m;
            var finalPrice = basePrice * (1 - discount);

            return finalPrice > 0 ? finalPrice : basePrice;
        }

        public static void RestorePrice(Product product)
        {
            if (product.OldPrice.HasValue && product.OldPrice.Value > 0)
                product.Price = product.OldPrice.Value;
            else
                product.Price = 1000m; // Giá mặc định tránh 0₫
            product.IsSale = false;
        }


        public static string DisplayPrice(decimal? price)
        {
            decimal display = (price.HasValue && price.Value > 0) ? price.Value : 0m;
            return string.Format("{0:N0} ₫", display);
        }
    }
}