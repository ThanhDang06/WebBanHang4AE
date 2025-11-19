using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WBH.Filters;
using WBH.Models;

namespace WBH.Controllers
{
    public class CustomersController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        // GET: Customers
        public ActionResult Index()
        {
            return View(db.Customers.ToList());
        }
        public ActionResult ProductList()
        {
            var products = db.Products.ToList();
            ViewBag.IsAdmin = false;
            return View(products);
        }
        // GET: Nhập voucher
        public ActionResult ApplyVoucher()
        {
            return View();
        }
        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var customer = db.Customers
                .FirstOrDefault(c => c.IDCus == id);

            if (customer == null)
                return HttpNotFound();

            var orders = db.Orders
                .Where(o => o.IDCus == id)
                .OrderByDescending(o => o.DateOrder)
                .Take(5)
                .ToList();

            ViewBag.Orders = orders;

            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Áp dụng voucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplyVoucher(ApplyVoucherViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (Session["IDCus"] == null)
                    return RedirectToAction("Login", "Account");

                int userId = Convert.ToInt32(Session["IDCus"]);

                var voucher = db.Vouchers
                    .FirstOrDefault(v => v.Code == model.Code
                                         && v.IsActive
                                         && v.StartDate <= DateTime.Now
                                         && v.EndDate >= DateTime.Now
                                         && v.RemainingUses > 0
                                         && (v.IDCus == null || v.IDCus == userId));

                if (voucher == null)
                {
                    ModelState.AddModelError("", "Voucher không hợp lệ hoặc đã hết hạn.");
                    return View(model);
                }

                if (model.OrderAmount < voucher.MinOrderAmount)
                {
                    ModelState.AddModelError("", $"Đơn hàng phải từ {voucher.MinOrderAmount:N0}₫ trở lên để áp dụng voucher.");
                    return View(model);
                }

                // Trừ số lần còn dùng
                voucher.RemainingUses -= 1;
                db.Entry(voucher).State = EntityState.Modified;
                db.SaveChanges();

                decimal discount = voucher.Type == "PERCENT"
                    ? model.OrderAmount * voucher.Value / 100
                    : voucher.Value;

                TempData["Success"] = $"Voucher hợp lệ! Bạn được giảm {discount:N0}₫";
                return RedirectToAction("Index", "Carts"); // hoặc trang bạn muốn hiển thị
            }

            return View(model);
        }
        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDCus,FullName,Email,Phone,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDCus,FullName,Email,Phone,Address")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Lấy entity từ DB
                var existingCustomer = db.Customers.Find(customer.IDCus);
                if (existingCustomer == null)
                {
                    return HttpNotFound();
                }

                // Cập nhật từng field
                existingCustomer.FullName = customer.FullName;
                existingCustomer.Email = customer.Email;
                existingCustomer.Phone = customer.Phone;
                existingCustomer.Address = customer.Address;

                db.SaveChanges();
                return RedirectToAction("CustomerManagement", "Admin");
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetOrderStatus(int id)
        {
            var order = db.Orders.Find(id);
            if (order != null)
                return Json(new { status = order.Status }, JsonRequestBehavior.AllowGet);

            return Json(new { status = "" }, JsonRequestBehavior.AllowGet);
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
