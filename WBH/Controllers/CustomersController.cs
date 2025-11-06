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
        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Tìm khách hàng theo ID tài khoản (IDAcc)
            var customer = db.Customers.FirstOrDefault(c => c.IDCus == id);

            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
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
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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

        public ActionResult Dashboard()
        {
            if (Session["Role"] == null || !Session["Role"].ToString().ToLower().Contains("customer"))
                return RedirectToAction("DangNhap", "Login");
            ViewBag.User = Session["UserName"];

            int customerId = Convert.ToInt32(Session["UserId"]);

            var orders = db.Orders
                .Where(o => o.IDCus == customerId)
                .OrderByDescending(o => o.DateOrder)
                .ToList();

            if (!orders.Any())
            {
                ViewBag.Message = "Bạn chưa có đơn hàng nào.";
                return View(new DashboardViewModel());
            }

            var viewModel = new DashboardViewModel
            {
                TotalOrders = orders.Count,
                TotalSpent = orders.Sum(o => o.Total ?? 0),
                PendingOrders = orders.Count(o => o.Status == "Đang xử lý" || o.Status == "Chờ xác nhận"),
                DeliveredOrders = orders.Count(o => o.Status == "Đã giao" || o.Status == "Hoàn thành"),
                LastOrderDate = orders.Max(o => o.DateOrder)
            };

            // Danh sách đơn gần nhất
            viewModel.RecentOrders = orders.Take(5).Select(o => new OrderSummaryItem
            {
                IDOrder = o.IDOrder,
                DateOrder = o.DateOrder ?? DateTime.MinValue,
                Status = o.Status,
                Total = o.Total ?? 0
            }).ToList();

            // Sản phẩm trong đơn mới nhất
            var latestOrder = orders.FirstOrDefault();
            if (latestOrder != null)
            {
                viewModel.LatestOrderProducts = db.OrderDetails
                    .Where(d => d.IDOrder == latestOrder.IDOrder)
                    .Select(d => new OrderProductItem
                    {
                        IDProduct = d.IDProduct ?? 0,
                        ProductName = d.Product.ProductName,
                        Quantity = d.Quantity ?? 0,
                        Price = d.Product.Price ?? 0
                    }).ToList();
            }

            // Biểu đồ chi tiêu theo tháng
            viewModel.MonthlySpending = orders
                .GroupBy(o => new { o.DateOrder.Value.Year, o.DateOrder.Value.Month })
                .OrderBy(g => g.Key.Year).ThenBy(g => g.Key.Month)
                .Select(g => new MonthlySpend
                {
                    MonthLabel = $"{g.Key.Month:D2}/{g.Key.Year}",
                    Amount = g.Sum(x => x.Total ?? 0)
                }).ToList();

            return View(viewModel);
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
