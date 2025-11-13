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
            int userId = Convert.ToInt32(Session["IDCus"]);
            var cartItems = db.Carts
                              .Include(c => c.Product) // load luôn Product
                              .Where(c => c.IDCus == userId)
                              .ToList();

            return View(cartItems);
        }
        //Thêm sản phẩm vào giỏ
        [HttpPost]
        public ActionResult AddToCart(int id, int colorId, int sizeId, int quantity = 1, string image = null)
        {
            if (Session["IDCus"] == null)
                return Json(new { success = false, message = "Vui lòng đăng nhập" });

            int userId = Convert.ToInt32(Session["IDCus"]);




            // Kiểm tra sản phẩm cùng màu + size
            var existing = db.Carts.FirstOrDefault(c => c.IDProduct == id
                                                      && c.IDCus == userId
                                                      && c.IDColor == colorId
                                                      && c.IDSize == sizeId);

            if (existing != null)
                existing.Quantity += quantity;
            else
                db.Carts.Add(new Cart
                {
                    IDCus = userId,
                    IDProduct = id,
                    Quantity = quantity,
                    IDColor = colorId,
                    IDSize = sizeId,
                    Image = db.ProductColors.Find(colorId)?.Image,
                    DateAdded = DateTime.Now
                });

            db.SaveChanges();
            return Json(new { success = true, message = "Đã thêm vào giỏ hàng" });
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
                Total = cartItems.Sum(x => x.Product.Price * x.Quantity)
            };

            db.Orders.Add(order);
            db.SaveChanges();

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

            db.Carts.RemoveRange(cartItems);
            db.SaveChanges();

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
