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
    public class OrdersController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        // GET: Orders
        public ActionResult Index(int? customerId)
        {
            IQueryable<Order> orders = db.Orders;

            if (customerId.HasValue)
            {
                orders = orders.Where(o => o.IDCus == customerId.Value);
            }

            orders = orders.OrderByDescending(o => o.DateOrder);

            return View(orders.ToList());
        }
        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var order = db.Orders
                          .Include(o => o.Customer)
                          .Include(o => o.OrderDetails.Select(od => od.Product))
                          .FirstOrDefault(o => o.IDOrder == id);

            if (order == null)
                return HttpNotFound();

            return View(order);
        }


        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IDOrder,IDCus,DateOrder,AddressDelivery,FullName,Phone,PaymentMethod,Note,Status,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", order.IDCus);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", order.IDCus);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDOrder,IDCus,DateOrder,AddressDelivery,FullName,Phone,PaymentMethod,Note,Status,Total")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.IDCus = new SelectList(db.Customers, "IDCus", "FullName", order.IDCus);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
