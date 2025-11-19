    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Entity;
    using System.Linq;
    using System.Net;
    using System.Web;
    using System.Web.Mvc;
    using WBH.Models;

    namespace WBH.Controllers
    {
        public class CartsController : Controller
        {
            private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        // GET: Carts
        public ActionResult Index()
        {
            // Kiểm tra đăng nhập
            if (Session["IDCus"] == null)
            {
                TempData["ReturnUrl"] = Url.Action("Index", "Cart");

                return RedirectToAction("DangNhap", "Login");
            }

            int userId = Convert.ToInt32(Session["IDCus"]);

            var cartItems = db.Carts.Include(c => c.Product)
                                    .Where(c => c.IDCus == userId)
                                    .ToList();

            var cartVM = new CartViewModel { Items = cartItems };

            // Lấy voucher từ DB để hiển thị
            ViewBag.Vouchers = db.Vouchers
                                 .Where(v => v.IsActive && v.StartDate <= DateTime.Now && v.EndDate >= DateTime.Now
                                             && (v.IDCus == null || v.IDCus == userId))
                                 .ToList();

            // Lấy voucher áp dụng từ Session
            if (Session["CartVoucherCode"] != null)
            {
                var code = Session["CartVoucherCode"]?.ToString();
                if (!string.IsNullOrEmpty(code))
                {
                    var applied = db.Vouchers.FirstOrDefault(v => v.Code == code);
                    if (applied != null)
                    {
                        cartVM.VoucherCode = applied.Code;
                        cartVM.Discount = applied.Type == "PERCENT"
                                          ? cartVM.TotalAmount * applied.Value / 100
                                          : applied.Value;
                    }
                }
            }

            return View(cartVM);
        }


        //Thêm sản phẩm vào giỏ
        [HttpPost]
        public ActionResult AddToCart(int id, int? colorId, int? sizeId, int quantity = 1, string image = null)
        {
            if (Session["IDCus"] == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            int userId = Convert.ToInt32(Session["IDCus"]);

            // Kiểm tra sản phẩm cùng màu + size (cho null nếu không có)
            var existing = db.Carts.FirstOrDefault(c =>
                c.IDProduct == id &&
                c.IDCus == userId &&
                (c.IDColor == colorId || colorId == null) &&
                (c.IDSize == sizeId || sizeId == null)
            );

            if (existing != null)
                existing.Quantity += quantity;
            else
            {
                string img = (colorId.HasValue && colorId.Value != 0
               ? db.ProductColors.Find(colorId)?.Image
               : null)
              ?? db.Products.Find(id)?.Image;


                db.Carts.Add(new Cart
                {
                    IDCus = userId,
                    IDProduct = id,
                    Quantity = quantity,
                    IDColor = colorId,
                    IDSize = sizeId,
                    Image = img,
                    DateAdded = DateTime.Now
                });
            }

            db.SaveChanges();
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng" });
        }


        [HttpPost]
        public JsonResult ApplyVoucher(string code)
        {
            var voucher = db.Vouchers.FirstOrDefault(v => v.Code == code);
            if (voucher == null)
                return Json(new { success = false, message = "Mã voucher không hợp lệ" });

            if (Session["IDCus"] == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            int userId = Convert.ToInt32(Session["IDCus"]);

            // Lấy giỏ hàng của user hiện tại
            var cartItems = db.Carts
                              .Where(c => c.IDCus == userId)
                              .Include(c => c.Product)
                              .ToList();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Giỏ hàng trống" });

            decimal totalAmount = cartItems.Sum(x => (x.Product.Price ?? 0) * x.Quantity);

            if (totalAmount < voucher.MinOrderAmount)
                return Json(new { success = false, message = $"Đơn hàng chưa đủ {voucher.MinOrderAmount:N0}₫ để áp dụng voucher" });

            decimal discount = 0;
            if (voucher.Type == "PERCENT")
                discount = totalAmount * voucher.Value / 100;
            else
                discount = voucher.Value;

            // Lưu voucher vào session để ghi nhớ
            Session["CartVoucherCode"] = code;

            return Json(new
            {
                success = true,
                discountValue = discount,
                discountText = discount.ToString("N0") + "₫",
                totalText = (totalAmount - discount).ToString("N0") + "₫"
            });
        }
        [HttpPost]
        public JsonResult RemoveVoucher()
        {
            Session["CartVoucherCode"] = null;
            return Json(new { success = true });
        }

        //Cập nhật số lượng
        [HttpPost]
            public ActionResult UpdateQuantity(int id, int quantity)
            {
                var cart = db.Carts.Find(id);
                if (cart != null)
                {
                    cart.Quantity = quantity;
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }
            // Xóa sản phẩm khỏi giỏ
            [HttpPost]
            public ActionResult Remove(int id)
            {
                var cartItem = db.Carts.Find(id);
                if (cartItem != null)
                {
                    db.Carts.Remove(cartItem);
                    db.SaveChanges();
                }
                return Json(new { success = true });
            }

        // Thanh toán
        [HttpPost]
        public JsonResult Checkout(string fullName, string phone, string address, string payment, string note, string citySelect, string districtSelect, string wardSelect)
        {
            int userId = Convert.ToInt32(Session["IDCus"]);
            var cartItems = db.Carts.Where(c => c.IDCus == userId).ToList();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Giỏ hàng trống!" });

            // Lấy voucher từ Session
            string voucherCode = Session["CartVoucherCode"]?.ToString();
            decimal discount = 0;

            if (!string.IsNullOrEmpty(voucherCode))
            {
                var voucher = db.Vouchers.FirstOrDefault(v => v.Code == voucherCode);
                if (voucher != null)
                {
                    discount = voucher.Type == "PERCENT"
                               ? cartItems.Sum(x => (x.Product.Price ?? 0) * x.Quantity) * voucher.Value / 100
                               : voucher.Value;
                }
            }

            // Tạo order với tổng tiền đã trừ voucher
            var totalAmount = cartItems.Sum(x => (x.Product.Price ?? 0) * x.Quantity);
            var order = new Order
            {
                IDCus = userId,
                FullName = fullName,
                Phone = phone,
                AddressDelivery = $"{address}, {wardSelect}, {districtSelect}, {citySelect}",
                PaymentMethod = payment,
                Note = note,
                DateOrder = DateTime.Now,
                Status = "Pending",
                Total = totalAmount, // tổng tiền gốc
                VoucherCode = voucherCode,
                Discount = discount
            };

            db.Orders.Add(order);
            db.SaveChanges();

            // Thêm chi tiết đơn hàng
            foreach (var item in cartItems)
            {
                db.OrderDetails.Add(new OrderDetail
                {
                    IDOrder = order.IDOrder,
                    IDProduct = item.IDProduct,
                    IDSize = item.IDSize,
                    IDColor = item.IDColor,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            db.SaveChanges();

            // Xóa giỏ hàng
            db.Carts.RemoveRange(cartItems);
            db.SaveChanges();

            // Xóa voucher khỏi session
            Session["CartVoucherCode"] = null;

            return Json(new { success = true, redirect = Url.Action("ThankYou") });
        }


        [HttpGet]
            public ActionResult GetCartItems()
            {
                if (Session["IDCus"] == null)
                    return Json(new object[] { }, JsonRequestBehavior.AllowGet);

                int userId = Convert.ToInt32(Session["IDCus"]);

                var items = db.Carts
                    .Where(c => c.IDCus == userId)
                    .Select(c => new
                    {
                        c.IDCart,
                        c.Product.ProductName,
                        Image = "/Content/Img/" + c.Image,
                        c.Quantity,
                        Price = c.Product.Price ?? 0,
                        OldPrice = c.Product.OldPrice ?? 0,
                        IsSale = c.Product.IsSale,
                        ColorName = c.ProductColor.ColorName,
                        SizeName = c.ProductSize.SizeName
                    })
                    .ToList();

                return Json(items, JsonRequestBehavior.AllowGet);
            }


            public ActionResult ThankYou() => View();
 
            // GET: Carts/Details/5
            public ActionResult Details(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Cart cart = db.Carts.Find(id);
                if (cart == null)
                {
                    return HttpNotFound();
                }
                return View(cart);
            }

            // GET: Carts/Create
            public ActionResult Create()
            {
                ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName");
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName");
                return View();
            }

            // POST: Carts/Create
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Create([Bind(Include = "IDCart,IDCus,IDProduct,Quantity,DateAdded")] Cart cart)
            {
                if (ModelState.IsValid)
                {
                    db.Carts.Add(cart);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", cart.IDCus);
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", cart.IDProduct);
                return View(cart);
            }

            // GET: Carts/Edit/5
            public ActionResult Edit(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Cart cart = db.Carts.Find(id);
                if (cart == null)
                {
                    return HttpNotFound();
                }
                ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", cart.IDCus);
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", cart.IDProduct);
                return View(cart);
            }

            // POST: Carts/Edit/5
            // To protect from overposting attacks, enable the specific properties you want to bind to, for 
            // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
            [HttpPost]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([Bind(Include = "IDCart,IDCus,IDProduct,Quantity,DateAdded")] Cart cart)
            {
                if (ModelState.IsValid)
                {
                    db.Entry(cart).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", cart.IDCus);
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", cart.IDProduct);
                return View(cart);
            }

            // GET: Carts/Delete/5
            public ActionResult Delete(int? id)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Cart cart = db.Carts.Find(id);
                if (cart == null)
                {
                    return HttpNotFound();
                }
                return View(cart);
            }

            // POST: Carts/Delete/5
            [HttpPost, ActionName("Delete")]
            [ValidateAntiForgeryToken]
            public ActionResult DeleteConfirmed(int id)
            {
                Cart cart = db.Carts.Find(id);
                db.Carts.Remove(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
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
