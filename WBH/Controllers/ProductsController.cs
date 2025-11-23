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

            // Gắn cờ hết hàng
            product.IsOutOfStock = product.Quantity <= 0;

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
            ViewBag.Colors = db.ProductColors.ToList();          // Lấy dữ liệu bảng Colors
            ViewBag.Sizes = db.ProductSizes.ToList();            // Nếu có bảng Sizes
            ViewBag.Categories = db.Products
                           .Select(p => p.Category)
                           .Distinct()
                           .ToList();

            return View();
        }

        // POST: Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
    [Bind(Include = "IDProduct,ProductName,Description,Price,Quantity,Category")] Product product,
    HttpPostedFileBase ImageFile,
    string CustomColors,
    HttpPostedFileBase[] ImageFiles, // Ảnh cho từng màu
    int[] SelectedSizes,
    string CustomSizes
)
        {
            // Giá mặc định
            if (!product.Price.HasValue || product.Price <= 0) product.Price = 1000m;
            if (!product.OldPrice.HasValue || product.OldPrice <= 0) product.OldPrice = product.Price;

            // Upload ảnh chung
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fileName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(ImageFile.FileName);
                string path = Server.MapPath("~/Content/Img/" + fileName);
                ImageFile.SaveAs(path);
                product.Image = fileName;
            }

            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();

                // Xử lý màu
                if (!string.IsNullOrEmpty(CustomColors))
                {
                    var colors = CustomColors.Split(',').Select(c => c.Trim()).ToList();
                    for (int i = 0; i < colors.Count; i++)
                    {
                        string colorImage = null;
                        if (ImageFiles != null && ImageFiles.Length > i && ImageFiles[i] != null && ImageFiles[i].ContentLength > 0)
                        {
                            string fName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(ImageFiles[i].FileName);
                            string path = Server.MapPath("~/Content/Img/" + fName);
                            ImageFiles[i].SaveAs(path);
                            colorImage = fName;
                        }

                        db.ProductColors.Add(new ProductColor
                        {
                            IDProduct = product.IDProduct,
                            ColorName = colors[i],
                            Image = colorImage ?? product.Image
                        });
                    }
                    db.SaveChanges();
                }

                // Xử lý size
                if (!string.IsNullOrEmpty(CustomSizes))
                {
                    var sizes = CustomSizes.Split(',').Select(s => s.Trim()).ToList();
                    foreach (var sizeName in sizes)
                    {
                        db.ProductSizes.Add(new ProductSize
                        {
                            IDProduct = product.IDProduct,
                            SizeName = sizeName
                        });
                    }
                    db.SaveChanges();
                }

                // Luôn redirect sau khi lưu xong
                return RedirectToAction("ProductManagement", "Admin");
            }

            // Nếu ModelState không hợp lệ, trả về view với dữ liệu cũ
            return View(product);
        }
        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
            {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products
                            .Include("ProductColors")
                            .Include("ProductSizes")
                            .FirstOrDefault(p => p.IDProduct == id);

            if (product == null)
                return HttpNotFound();

            // Danh sách màu & size
            ViewBag.Colors = db.ProductColors.ToList();
            ViewBag.Sizes = db.ProductSizes.ToList();

            // Lưu mảng màu và size đã chọn để hiển thị pre-selected
            ViewBag.SelectedColors = product.ProductColors.Select(pc => pc.IDColor).ToArray();
            ViewBag.SelectedSizes = product.ProductSizes.Select(ps => ps.IDSize).ToArray();

            return View(product);
        }

            // POST: Products/Edit/5
            [HttpPost]
            [ValidateAntiForgeryToken]
        public ActionResult Edit(
    [Bind(Include = "IDProduct,ProductName,Description,Price,Quantity,Category")] Product product,
    HttpPostedFileBase ImageFile,
    string CustomColors,
    HttpPostedFileBase[] ImageFiles,
    string CustomSizes
)
        {
            var productInDb = db.Products
                .Include("ProductColors")
                .Include("ProductSizes")
                .FirstOrDefault(p => p.IDProduct == product.IDProduct);

            if (productInDb == null) return HttpNotFound();

            // Cập nhật thông tin cơ bản
            productInDb.ProductName = product.ProductName;
            productInDb.Description = product.Description;
            productInDb.Price = product.Price;
            productInDb.Quantity = product.Quantity;
            productInDb.Category = product.Category;

            // Cập nhật ảnh chung
            if (ImageFile != null && ImageFile.ContentLength > 0)
            {
                string fName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(ImageFile.FileName);
                string path = Server.MapPath("~/Content/Img/" + fName);
                ImageFile.SaveAs(path);
                productInDb.Image = fName;
            }

            // Xử lý màu sản phẩm
            string[] colors = !string.IsNullOrEmpty(CustomColors)
                ? CustomColors.Split(',').Select(c => c.Trim()).Where(c => !string.IsNullOrEmpty(c)).ToArray()
                : new string[0];

            for (int i = 0; i < colors.Length; i++)
            {
                string colorName = colors[i];
                string colorImage = (ImageFiles != null && ImageFiles.Length > i && ImageFiles[i] != null && ImageFiles[i].ContentLength > 0)
                                    ? SaveFileAndReturnName(ImageFiles[i])
                                    : productInDb.Image; // fallback ảnh chung

                var existingColor = productInDb.ProductColors.FirstOrDefault(c => c.ColorName == colorName);
                if (existingColor != null)
                {
                    existingColor.Image = colorImage; // cập nhật ảnh nếu có
                }
                else
                {
                    db.ProductColors.Add(new ProductColor
                    {
                        IDProduct = product.IDProduct,
                        ColorName = colorName,
                        Image = colorImage
                    });
                }
            }

            // Xử lý size sản phẩm
            string[] sizes = !string.IsNullOrEmpty(CustomSizes)
                ? CustomSizes.Split(',').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)).ToArray()
                : new string[0];

            foreach (var sizeName in sizes)
            {
                int sizeId = GetSizeIdByName(sizeName); // Bạn cần implement mapping từ tên size -> IDSize
                if (!productInDb.ProductSizes.Any(s => s.IDSize == sizeId))
                {
                    db.ProductSizes.Add(new ProductSize
                    {
                        IDProduct = product.IDProduct,
                        IDSize = sizeId
                    });
                }
            }

            db.SaveChanges();

            return RedirectToAction("ProductManagement", "Admin");
        }

        private string SaveFileAndReturnName(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0) return null;
            string fName = DateTime.Now.Ticks + "_" + System.IO.Path.GetFileName(file.FileName);
            string path = Server.MapPath("~/Content/Img/" + fName);
            file.SaveAs(path);
            return fName;
        }

        private int GetSizeIdByName(string sizeName)
        {
            var size = db.ProductSizes.FirstOrDefault(s => s.SizeName == sizeName.Trim());
            if (size != null) return size.IDSize;

            var newSize = new ProductSize { SizeName = sizeName.Trim() };
            db.ProductSizes.Add(newSize);
            db.SaveChanges();
            return newSize.IDSize;
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var product = db.Products
                            .Include("ProductColors")
                            .Include("ProductSizes")
                            .FirstOrDefault(p => p.IDProduct == id);

            if (product == null)
                return HttpNotFound();

            return View(product);
        }
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var product = db.Products
                            .Include("ProductColors")
                            .Include("ProductSizes")
                            .FirstOrDefault(p => p.IDProduct == id);

            if (product != null)
            {
                // Xóa cả màu và size liên kết (nếu muốn)
                db.ProductColors.RemoveRange(product.ProductColors);
                db.ProductSizes.RemoveRange(product.ProductSizes);

                // Xóa sản phẩm
                db.Products.Remove(product);
                db.SaveChanges();
            }

            return RedirectToAction("ProductManagement", "Admin");
        }

        // Lấy danh sách sản phẩm cho Admin / ProductList / Details
        public ActionResult ProductList(string sortOrder)
        {
            var today = DateTime.Today;

            // 1️⃣ Lấy sản phẩm không đang sale
            var products = db.Products
                .Where(p => db.Sales
                    .Where(s => s.IDProduct == p.IDProduct)
                    .All(s => s.StartDate > today || s.EndDate < today))
                .ToList();

            // 2️⃣ Lấy danh sách màu cho từng sản phẩm
            var displayProducts = products.Select(p => new Product
            {
                IDProduct = p.IDProduct,
                ProductName = p.ProductName,
                Price = p.Price,
                OldPrice = p.OldPrice,
                IsSale = p.IsSale,
                Category = p.Category,
                Image = p.Image,
                Description = p.Description,
                Quantity = p.Quantity,
                ProductColors = db.ProductColors
                    .Where(pc => pc.IDProduct == p.IDProduct)
                    .ToList(),
                IsOutOfStock = p.Quantity <= 0

            }).ToList();

            // 3️⃣ Sắp xếp
            switch (sortOrder)
            {
                case "price_asc":
                    displayProducts = displayProducts.OrderBy(p => p.Price ?? 0).ToList();
                    break;
                case "price_desc":
                    displayProducts = displayProducts.OrderByDescending(p => p.Price ?? 0).ToList();
                    break;
                case "name_asc":
                    displayProducts = displayProducts.OrderBy(p => p.ProductName).ToList();
                    break;
                case "name_desc":
                    displayProducts = displayProducts.OrderByDescending(p => p.ProductName).ToList();
                    break;
                default:
                    displayProducts = displayProducts.OrderBy(p => p.IDProduct).ToList();
                    break;
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
