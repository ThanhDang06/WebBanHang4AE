using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBH.Models;


namespace WBH.Controllers
{

    public class AdminController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();
        // GET: Admin
        public ActionResult Admin()
        {
            // Kiểm tra role
            if (Session["AccountId"] == null || Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("DangNhap", "Login"); // chưa login hoặc không phải admin

            return View(); // view Admin/Admin.cshtml
        }
        public ActionResult ProductList()
        {
            var products = db.Products.ToList();
            ViewBag.IsAdmin = true;
            return View(products);
        }
    }
}