using System.Linq;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class LoginController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();

        // GET: Login
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(string username, string password)
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
                return View();
            }

            // Lưu thông tin đăng nhập vào session
            Session["UserName"] = account.Username;
            Session["Role"] = account.Role;
            Session["IDAcc"] = account.IDAcc;

            // Chuyển hướng theo vai trò
            switch (account.Role)
            {
                case "Admin":
                    return RedirectToAction("Dashboard", "Admin");
                case "Customer":
                    return RedirectToAction("Dashboard", "Customers");
                default:
                    return RedirectToAction("DangNhap");
            }
        }

        public ActionResult DangXuat()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("DangNhap");

        }

    }
}