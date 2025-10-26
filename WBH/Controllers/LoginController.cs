using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class LoginController : Controller
    {
        private DBFashionStoreEntities db = new DBFashionStoreEntities();

        // GET: Login
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public ActionResult DangNhap(string username, string password)
        {
            var account = db.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password);

            if (account != null)
            {
                Session["AccountID"] = account.IDAcc;
                Session["Username"] = account.Username;
                Session["Role"] = account.Role;

                // Phân quyền
                if (account.Role == "Admin")
                {
                    return RedirectToAction("Admin", "Admin"); // chuyển tới trang Admin
                }
                else if (account.Role == "Customer")
                {
                    // Lấy Id customer tương ứng với account login
                    var customer = db.Customers.FirstOrDefault(c => c.IDCus == account.IDAcc);
                    if (customer != null)
                        return RedirectToAction("Details", "Customers", new { id = customer.IDCus });
                    else
                        return RedirectToAction("Index", "WBH"); // fallback
                }
                else
                {
                    ViewBag.Error = "Role không hợp lệ!";
                    return View();
                }
            }

            ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
            return View();
        }

        // Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("DangNhap");
        }

    }
}