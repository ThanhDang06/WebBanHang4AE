using System.Web.Mvc;
using WBH.Models;

namespace WBH.Controllers
{
    public class WBHController : Controller
    {
        private DBFashionStoreEntitiess db = new DBFashionStoreEntitiess();
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
            return View();
        }
    }
}