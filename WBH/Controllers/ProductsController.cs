using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class ProductsController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();

        // GET: Products
        public ActionResult Index()
        {
            return View(db.Products.ToList());
        }
        // GET: Products/Sale
        public ActionResult Sale()
        {
            var today = DateTime.Today;
            var saleProducts = db.Products
                .Where(p => db.Sales.Any(s => s.IDProduct == p.IDProduct
                                            && s.StartDate <= today
                                            && s.EndDate >= today))
                .ToList();
            return View(saleProducts);
        }
        public ActionResult CategoryProducts(string category, string sortOrder)
        {
            if (string.IsNullOrEmpty(category))
                return RedirectToAction("ProductList");
            category = HttpUtility.UrlDecode(category);
            var products = db.Products.Where(p => p.Category == category);

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.IDProduct); // mặc định
                    break;
                
            }
            ViewBag.CurrentCategory = category;
            ViewBag.SortOrder = sortOrder;
            return View(products.ToList());
        }
        // GET: Products/Details/5
        public ActionResult Details(int? id)
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
            if (ModelState.IsValid)
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            if (ModelState.IsValid)
            {
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
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
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult ProductList(string sortOrder)
        {
            var products = db.Products.AsQueryable();

            // Sắp xếp an toàn với nullable
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price ?? 0); // null -> 0
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price ?? 0);
                    break;
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                default:
                    products = products.OrderBy(p => p.IDProduct);
                    break;
            }
            return View(products.ToList());
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
