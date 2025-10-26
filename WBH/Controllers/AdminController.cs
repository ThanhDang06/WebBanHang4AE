using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WBH.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Admin()
        {
            // Kiểm tra role
            if (Session["AccountId"] == null || Session["Role"]?.ToString() != "Admin")
                return RedirectToAction("DangNhap", "Login"); // chưa login hoặc không phải admin

            return View(); // view Admin/Admin.cshtml
        }
    }
}