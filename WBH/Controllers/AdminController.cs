using System.Linq;
using System.Web.Mvc;
using WBH.Models;


namespace WBH.Controllers
{

    public class AdminController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();
        // GET: Admin
        public ActionResult Dashboard()
        {
            if (Session["Role"] == null || Session["Role"].ToString() != "Admin")
                return RedirectToAction("DangNhap", "Login");

            // Tổng số liệu:
            decimal? totalRevenue = db.Orders.Sum(o => (decimal?)o.Total);
            ViewBag.TotalRevenue = totalRevenue.HasValue ? totalRevenue.Value : 0;
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
        public ActionResult ProductList()
        {
            var products = db.Products.ToList();
            ViewBag.IsAdmin = true;
            return View(products);
        }

    }
}