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
    public class AdminVouchersController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        // GET: AdminVouchers
        public ActionResult Index()
        {
            return View(db.Vouchers.ToList());
        }

        // GET: AdminVouchers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // GET: AdminVouchers/Create
        public ActionResult Create()
        {
            ViewBag.Customers = new SelectList(db.Customers, "IDCus", "FullName");
            return View();
        }

        private string GenerateRandomCode(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        // POST: AdminVouchers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateVoucherViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Sinh mã voucher ngẫu nhiên 8 ký tự
                string code = GenerateRandomCode(8);

                var voucher = new Voucher
                {
                    Code = code,
                    Type = model.Type,
                    Value = model.Value,
                    MinOrderAmount = model.MinOrderAmount,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    RemainingUses = model.RemainingUses,
                    IsActive = true,
                    CreatedDate = DateTime.Now,
                    IDCus = model.IDCus
                };

                db.Vouchers.Add(voucher);
                db.SaveChanges();

                TempData["Success"] = $"Voucher đã tạo: {code}";
                return RedirectToAction("Create");
            }

            ViewBag.Customers = new SelectList(db.Customers, "IDCus", "FullName", model.IDCus);
            return View(model);
        }

        // GET: AdminVouchers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                TempData["Error"] = "ID voucher không hợp lệ.";
                return RedirectToAction("Index");
            }

            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                TempData["Error"] = "Voucher không tồn tại.";
                return RedirectToAction("Index");
            }

            return View(voucher);
        }

        // POST: AdminVouchers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IDVoucher,Code,Type,Value,MinOrderAmount,StartDate,EndDate,RemainingUses,IsActive")] Voucher voucher)
        {
            if (ModelState.IsValid)
            {
                // Load bản ghi từ DB
                var existingVoucher = db.Vouchers.Find(voucher.IDVoucher);
                if (existingVoucher == null)
                {
                    return HttpNotFound(); // Hoặc báo lỗi
                }

                // Cập nhật các trường
                existingVoucher.Code = voucher.Code;
                existingVoucher.Type = voucher.Type;
                existingVoucher.Value = voucher.Value;
                existingVoucher.MinOrderAmount = voucher.MinOrderAmount;
                existingVoucher.StartDate = voucher.StartDate;
                existingVoucher.EndDate = voucher.EndDate;
                existingVoucher.RemainingUses = voucher.RemainingUses;
                existingVoucher.IsActive = voucher.IsActive;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(voucher);
        }


        // GET: AdminVouchers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Voucher voucher = db.Vouchers.Find(id);
            if (voucher == null)
            {
                return HttpNotFound();
            }
            return View(voucher);
        }

        // POST: AdminVouchers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Voucher voucher = db.Vouchers.Find(id);
            db.Vouchers.Remove(voucher);
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
