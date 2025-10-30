using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class CartsController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();

        // GET: Carts
        public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.Customer).Include(c => c.Product);
            return View(carts.ToList());
        }

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
        // GET: Carts/MyCart
        public ActionResult MyCart(int idCus = 1) // idCus tạm = 1
        {
            // Lấy ID khách hàng hiện tại từ session
            int currentUserId = 0;
            if (Session["UserId"] != null)
            {
                currentUserId = (int)Session["UserId"];
            }
            else
            {
                // Nếu chưa login thì chuyển hướng về trang đăng nhập
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách giỏ hàng của khách hiện tại
            var cartItems = db.Carts
                              .Where(c => c.IDCus == currentUserId)
                              .Include(c => c.Product)
                              .ToList();

            return View(cartItems);
        }

        // GET: Carts/Checkout
        public ActionResult Checkout(int idCus = 1)
        {
            var carts = db.Carts.Where(c => c.IDCus == idCus).ToList();
            if (carts.Any())
            {
                db.Carts.RemoveRange(carts);
                db.SaveChanges();
            }
            TempData["Success"] = "Thanh toán thành công!";
            return RedirectToAction("MyCart", new { idCus = idCus });
        }
    }
}
