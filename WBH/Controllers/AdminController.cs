using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{

    public class AdminController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();
        // GET: Admin
        public ActionResult Dashboard()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("DangNhap", "Login");

            // Tổng số liệu:
            decimal? totalRevenue = db.Orders.Sum(o => (decimal?)o.Total);// Sử dụng decimal? để tránh lỗi null
            ViewBag.TotalRevenue = totalRevenue.HasValue ? totalRevenue.Value : 0; // Nếu null thì gán 0
            ViewBag.TotalOrders = db.Orders.Count();
            ViewBag.TotalCustomers = db.Customers.Count();
            ViewBag.TotalProducts = db.Products.Count();

            // Biểu đồ doanh thu theo tháng
            var data = db.Orders
                .Where(o => o.DateOrder.HasValue)
                .GroupBy(o => new { Year = o.DateOrder.Value.Year, Month = o.DateOrder.Value.Month })
                .Select(g => new
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    Revenue = g.Sum(o => (decimal?)o.Total) ?? 0
                })
                .OrderBy(g => g.Year).ThenBy(g => g.Month)
                .ToList();

            ViewBag.CharData = data;

            ViewBag.User = Session["UserName"];
            return View();

        }
        // GET: Admin/RepairProducts
        public ActionResult RepairProducts()
        {
            var products = db.Products.ToList();
            var today = DateTime.Today;

            foreach (var p in products)
            {
                // Nếu Price null hoặc <= 0 → gán mặc định 1000₫
                if (!p.Price.HasValue || p.Price.Value <= 0)
                    p.Price = 1000m;

                // Nếu OldPrice null hoặc <= 0 → gán bằng Price
                if (!p.OldPrice.HasValue || p.OldPrice.Value <= 0)
                    p.OldPrice = p.Price;

                // Kiểm tra sale còn hiệu lực không
                var activeSale = db.Sales.FirstOrDefault(s => s.IDProduct == p.IDProduct &&
                                                             s.StartDate <= today && s.EndDate >= today);
                p.IsSale = activeSale != null;

                db.Entry(p).State = EntityState.Modified;
            }

            db.SaveChanges();

            TempData["Message"] = "Đã cập nhật tất cả sản phẩm thành công!";
            return RedirectToAction("ProductList", "Products");
        }

        // GET: Admin/Orders
        public ActionResult Orders()
        {
            var orders = db.Orders
                .OrderByDescending(o => o.DateOrder)
                .ToList();
            return View(orders);
        }

        // GET: Admin/OrderDetails/5
        public ActionResult OrderDetails(int id)
        {
            var order = db.Orders
                 .Include("OrderDetails.Product")
                 .Include("OrderDetails.ProductColor")
                 .Include("OrderDetails.ProductSize")
                 .FirstOrDefault(o => o.IDOrder == id);


            if (order == null) return HttpNotFound();

            return View(order);
        }

        public ActionResult ProductList()
        {
            var products = db.Products.ToList();
            ViewBag.IsAdmin = true;
            return View(products);
        }
        public ActionResult ProductManagement()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        public ActionResult CustomerManagement()
        {
            var customers = db.Customers.ToList();
            return View(customers);
        }
        [HttpPost]
        public JsonResult UpdateOrderStatus(int id, string status)
        {
            var order = db.Orders.Find(id);
            if (order != null)
            {
                order.Status = status;
                db.SaveChanges();
                return Json(new { success = true, status = order.Status });
            }
            return Json(new { success = false });
        }
    }
}