using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class WBHController : Controller
    {
        private DBFashionStoreEntities01 db = new DBFashionStoreEntities01();
        // GET: WBH
        public ActionResult Home()
        {
            return View();
        }
        public ActionResult Sanpham()
        {
            return View();
        }
        public ActionResult Sale()
        {
            return View();
        }
        public ActionResult Phukien()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Giohang()
        {
            return View();
        }
        public ActionResult Chitietsp()
        {
            return View();
        }
        public ActionResult Thanhtoan()
        {
            return View();
        }
        public ActionResult DangNhap()
        {
            if (Session["AccountId"] == null)
                return RedirectToAction("Index", "Login"); // chưa login → chuyển về login

            return View();
        }
    }
}