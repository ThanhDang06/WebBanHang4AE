using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using WBH.Models;

namespace WBH.Controllers
{
    public class LoginController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();

        [HttpGet]
        public ActionResult DangNhap(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password, string returnUrl)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Vui lòng nhập đầy đủ thông tin!";
                return View();
            }

            var account = db.Accounts.FirstOrDefault(a => a.Username == username && a.Password == password);
            if (account == null)
            {
                ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                return View();// Sai thông tin đăng nhập
            }

            // Tạo authentication ticket
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                account.Role,
                FormsAuthentication.FormsCookiePath // Đường dẫn cookie
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);// Mã hóa ticket
            HttpCookie authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);// Tạo cookie
            Response.Cookies.Add(authCookie);// Thêm cookie vào response

            // Lưu session
            Session["UserName"] = account.Username;
            Session["Role"] = account.Role;
            Session["IDAcc"] = account.IDAcc;

            // Lưu IDCus nếu là khách hàng
            if (account.Role != "Admin")
            {
                var customer = db.Customers.FirstOrDefault(c => c.IDAcc == account.IDAcc);
                if (customer != null)
                    Session["IDCus"] = customer.IDCus;
            }

            if (TempData["ReturnUrl"] != null)
            {
                string url = TempData["ReturnUrl"].ToString();
                return Redirect(url);
            }

            // Redirect theo role
            if (account.Role == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else
            {
                var customer = db.Customers.FirstOrDefault(c => c.IDAcc == account.IDAcc);
                if (customer != null)
                {
                    Session["IDCus"] = customer.IDCus; // đảm bảo session có IDCus
                    return RedirectToAction("ProductList", "Products", new { id = customer.IDCus });
                }
                else
                {
                    ViewBag.Error = "Không tìm thấy thông tin khách hàng!";
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult DangKy()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangKy(string username, string password, string email, string fullname, string phone, string address)
        {
            // Validate dữ liệu
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Vui lòng điền đầy đủ thông tin!";
                return View();
            }

            if (db.Accounts.Any(a => a.Username == username))
            {
                ViewBag.Error = "Tên tài khoản đã tồn tại!";
                return View();
            }

            if (db.Customers.Any(c => c.Email == email))
            {
                ViewBag.Error = "Email đã được đăng ký!";
                return View();
            }

            // Tạo account mới
            var account = new Account
            {
                Username = username,
                Password = password,
                Role = "Customer"
            };
            db.Accounts.Add(account);
            db.SaveChanges();

            // Tạo customer mới
            var customer = new Customer
            {
                IDAcc = account.IDAcc,
                FullName = fullname,
                Email = email,
                Phone = phone,
                Address = address
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            // Tự động login sau khi đăng ký
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                username,
                DateTime.Now,
                DateTime.Now.AddHours(1),
                false,
                account.Role,
                FormsAuthentication.FormsCookiePath
            );
            string encryptedTicket = FormsAuthentication.Encrypt(ticket);
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket));

            Session["UserName"] = account.Username;
            Session["Role"] = account.Role;
            Session["IDAcc"] = account.IDAcc;
            Session["IDCus"] = customer.IDCus;

            return RedirectToAction("ProductList", "Products");
        }

        // ================= Logout =================
        public ActionResult DangXuat()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("DangNhap");
        }

    }
}