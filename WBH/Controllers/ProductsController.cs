    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using WBH.Helpers;
    using WBH.Models;

    namespace WBH.Controllers
    {
        public class ProductsController : Controller
        {
            private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

            // GET: Products
            public ActionResult ClothesList(string sortOrder = "")
            {
                var products = db.Products
                                 .Where(p => p.Category == "ao" || p.Category == "quan")
                                 .AsQueryable();

                products = SortHelper.ApplySort(products, sortOrder);
                return View(products.ToList());
            }

            public ActionResult AccessoriesList(string sortOrder = "")
            {
                var products = db.Products
                                .Where(p => p.Category == "vo" || p.Category == "non" || p.Category == "trangsuc")
                                .AsQueryable();
            products = SortHelper.ApplySort(products, sortOrder);
            return View(products.ToList());
            }
        // GET: Products/Sale
        public ActionResult Sale()
        {
            var today = DateTime.Today;

            var saleProducts = (from p in db.Products
                                join s in db.Sales
                                    on p.IDProduct equals s.IDProduct
                                where s.StartDate <= today && s.EndDate >= today
                                select new { Product = p, Sale = s })
                    .AsEnumerable()
                    .Select(x =>
                    {
                        var product = x.Product;
                        if (!product.OldPrice.HasValue)
                            product.OldPrice = product.Price ?? 1000m;
                        var displayProduct = new Product
                        {
                            IDProduct = product.IDProduct,
                            ProductName = product.ProductName,
                            Image = product.Image,
                            Price = SaleHelper.GetSalePrice(product, (decimal)x.Sale.DiscountPercent),
                            OldPrice = product.OldPrice ?? product.Price,
                            IsSale = true
                        };
                        return displayProduct;
                    }).ToList();


            return View(saleProducts);
        }

        public ActionResult CategoryProducts(string category, string sortOrder)
            {
                if (string.IsNullOrEmpty(category))
                    return RedirectToAction("ProductList");

            if (!db.Products.Any(p => p.Category == category))
                return HttpNotFound();

            category = HttpUtility.UrlDecode(category);
                var products = db.Products.Where(p => p.Category == category);

            products = SortHelper.ApplySort(products, sortOrder);

            ViewBag.CurrentCategory = category;
                ViewBag.SortOrder = sortOrder;   
                return View(products.ToList());
            }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products.Find(id);
            if (product == null)
                return HttpNotFound();

            if (product.IsSale && product.OldPrice.HasValue && product.Price.HasValue)
            {
                ViewBag.OldPrice = product.OldPrice.Value;
                ViewBag.CurrentPrice = product.Price.Value;

                // Tính phần trăm giảm (sửa lỗi decimal?)
                decimal percent = Math.Round(
                    ((product.OldPrice.Value - product.Price.Value) / product.OldPrice.Value) * 100,
                    0
                );
                ViewBag.SalePercent = percent;
            }
            else
            {
                ViewBag.OldPrice = null;
                ViewBag.CurrentPrice = product.Price;
                ViewBag.SalePercent = null;
            }

            return View(product);
        }




        // GET: Products/Create
        public ActionResult Create()
            {
                return View();
            }

            // POST: Products/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDProduct,ProductName,Description,Price,Quantity,Image,Category")] Product product)
        {
            if (!product.Price.HasValue || product.Price <= 0)
                product.Price = 1000m;

            if (!product.OldPrice.HasValue || product.OldPrice <= 0)
                product.OldPrice = product.Price;

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }

            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

            // POST: Products/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "IDProduct,ProductName,Description,Price,Quantity,Image,Category")] Product product)
            {

            if (!product.Price.HasValue || product.Price <= 0)
                product.Price = 1000m;

            if (!product.OldPrice.HasValue || product.OldPrice <= 0)
                product.OldPrice = product.Price;

            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ProductList");
            }

            return View(product);
            }

            // GET: Products/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Product product = db.Products.Find(id);
                if (product == null)
                {
                    return HttpNotFound();
                }
                return View(product);
            }

            // POST: Products/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Product product = db.Products.Find(id);
                if (product != null)
                {
                    db.Products.Remove(product);
                    db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

        // Lấy danh sách sản phẩm cho Admin / ProductList / Details
        public ActionResult ProductList(string sortOrder)
        {
            var today = DateTime.Today;

            var products = db.Products
                .Include(p => p.ProductColors)
                .Where(p => !db.Sales.Any(s =>
                    s.IDProduct == p.IDProduct &&
                    s.StartDate <= today &&
                    s.EndDate >= today))
                .ToList();


            // Tính giá hiển thị tạm thời (không lưu DB)
            var displayProducts = products.Select(p => new Product
            {
                IDProduct = p.IDProduct,
                ProductName = p.ProductName,
                Price = p.IsSale && p.OldPrice.HasValue
                        ? p.Price // đã giảm rồi
                        : p.Price, // giá gốc
                OldPrice = p.OldPrice,
                IsSale = p.IsSale,
                Category = p.Category,
                Image = p.Image,
                Description = p.Description,
                Quantity = p.Quantity,
                ProductColors = p.ProductColors
            }).ToList();

            // Sắp xếp
            switch (sortOrder)
            {
                case "price_asc": displayProducts = displayProducts.OrderBy(p => p.Price ?? 0).ToList(); break;
                case "price_desc": displayProducts = displayProducts.OrderByDescending(p => p.Price ?? 0).ToList(); break;
                case "name_asc": displayProducts = displayProducts.OrderBy(p => p.ProductName).ToList(); break;
                case "name_desc": displayProducts = displayProducts.OrderByDescending(p => p.ProductName).ToList(); break;
                default: displayProducts = displayProducts.OrderBy(p => p.IDProduct).ToList(); break;
            }

            ViewBag.IsAdmin = Session["Role"] != null && Session["Role"].ToString() == "Admin";
            return View(displayProducts);
        }


        [HttpGet]
        [AllowAnonymous]
        public JsonResult SearchAjax(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var today = DateTime.Today;
            string cleanKeyword = keyword.ToLower();

            var products = (from p in db.Products
                            join s in db.Sales
                                .Where(s => s.StartDate <= today && s.EndDate >= today)
                                on p.IDProduct equals s.IDProduct into ps
                            from s in ps.DefaultIfEmpty()
                            where p.ProductName.ToLower().Contains(cleanKeyword)
                            select new
                            {
                                p.IDProduct,
                                p.ProductName,
                                p.Image,
                                OriginalPrice = (double)(p.Price ?? 0),
                                SalePercent = s != null ? (double?)s.DiscountPercent : null
                            })
                        .Take(8)
                        .ToList();

            var result = products.Select(p => new
            {
                p.IDProduct,
                p.ProductName,
                Image = Url.Content("~/Content/Img/" + p.Image),
                OriginalPrice = p.OriginalPrice,
                SalePrice = p.SalePercent.HasValue
                            ? Math.Round(p.OriginalPrice * (1 - p.SalePercent.Value / 100), 0)
                            : (double?)null,
                SaleLabel = p.SalePercent.HasValue ? $"-{p.SalePercent.Value}%" : null
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }



        public ActionResult Search(string keyword)
        {
            var today = DateTime.Today;

            var products = (from p in db.Products
                            join s in db.Sales
                                on p.IDProduct equals s.IDProduct into ps
                            from s in ps.DefaultIfEmpty()
                            where p.ProductName.Contains(keyword)
                            select new { p, s })
                            .AsEnumerable()
                            .Select(x =>
                            {
                                var p = x.p;
                                var s = x.s;

                                // Xác định có đang sale hợp lệ không
                                bool isOnSale = s != null && s.StartDate <= today && s.EndDate >= today;

                                // Giá giảm (nếu có)
                                decimal? salePrice = null;
                                if (isOnSale && s.DiscountPercent.HasValue)
                                {
                                    salePrice = Math.Round((p.Price ?? 0m) * (1 - s.DiscountPercent.Value / 100), 0);
                                }

                                return new Product
                                {
                                    IDProduct = p.IDProduct,
                                    ProductName = p.ProductName,
                                    Image = p.Image,
                                    Price = salePrice ?? p.Price, // giá hiển thị
                                    OldPrice = salePrice.HasValue ? p.Price : (decimal?)null, // giá gốc hiển thị gạch ngang
                                    IsSale = salePrice.HasValue
                                };
                            }).ToList();

            return View(products);
        }

        protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    db.Dispose();
                }
                base.Dispose(disposing);
            }

        }
    }
