using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WBH.Helpers;
using WBH.Models;

namespace WBH.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SalesController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        // GET: Sales
        public ActionResult Index()
        {
            var sales = db.Sales.Include(s => s.Product)
                                .Where(s => s.Product != null)
                                .ToList();
            return View(sales);
        }


        // GET: Sales/Create
        public ActionResult Create()
        {
            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName");
            return View();
        }

        // POST: Sales/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDSale,IDProduct,Category,DiscountPercent,StartDate,EndDate")] Sale sale)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", sale.IDProduct);
                return View(sale);
            }
            var product = db.Products.Find(sale.IDProduct);
            if (product != null)
            {
                if (product.OldPrice == null || product.OldPrice == 0)
                    product.OldPrice = product.Price; // lưu lại giá gốc 1 lần duy nhất

                product.Price = SaleHelper.GetSalePrice(product, sale.DiscountPercent);
                product.IsSale = true;

                db.Entry(product).State = EntityState.Modified;
            }


            // Chỉ thêm sale, không thay đổi giá Product
            db.Sales.Add(sale);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Sales/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sale = db.Sales.Include(s => s.Product).FirstOrDefault(s => s.IDSale == id);
            if (sale == null) return HttpNotFound();

            return View(sale);
        }
        // GET: Sales/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var sale = db.Sales.Find(id);
            if (sale == null) return HttpNotFound();

            ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", sale.IDProduct);
            return View("Edit", sale);
        }

        // POST: Sales/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDSale,IDProduct,DiscountPercent,StartDate,EndDate")] Sale sale)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.IDProduct = new SelectList(db.Products, "IDProduct", "ProductName", sale.IDProduct);
                return View(sale);
            }

            var saleInDb = db.Sales.Find(sale.IDSale);
            if (saleInDb == null) return HttpNotFound();

            // Chỉ cập nhật những trường hợp hợp lệ
            saleInDb.DiscountPercent = sale.DiscountPercent;
            saleInDb.StartDate = sale.StartDate;
            saleInDb.EndDate = sale.EndDate;

            if (saleInDb.IDProduct != null)
            {
                var product = db.Products.Find(saleInDb.IDProduct);
                if (product != null)
                {
                    product.Price = SaleHelper.GetSalePrice(product, saleInDb.DiscountPercent);
                    product.IsSale = true;
                    db.Entry(product).State = EntityState.Modified;
                }
            }
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: Sales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var sale = db.Sales.Find(id);
            if (sale != null)
            {
                var product = db.Products.Find(sale.IDProduct);
                if (product != null)
                {
                    // Khôi phục lại giá gốc
                    product.Price = product.OldPrice ?? 1000m;
                    product.IsSale = false;

                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                }

                db.Sales.Remove(sale);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
