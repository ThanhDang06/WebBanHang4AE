using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace WBH.Controllers
{
    public class LocationController : Controller
    {
        private static readonly HttpClient client = new HttpClient();

        // GET: Location/Cities
        public async Task<ActionResult> Cities()
        {
            try
            {
                string apiUrl = "https://vapi.vnappmob.com/api/v2/province/";
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch
            {
                // Log ex.Message nếu muốn
                return new HttpStatusCodeResult(500, "Lỗi khi lấy dữ liệu Tỉnh/Thành phố");
            }
        }

        // GET: Location/Districts/92  (92 là id tỉnh)
        public async Task<ActionResult> Districts(int id)
        {
            try
            {
                string apiUrl = $"https://vapi.vnappmob.com/api/v2/province/district/{id}";
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch
            {
                return new HttpStatusCodeResult(500, "Lỗi khi lấy dữ liệu Quận/Huyện");
            }
        }

        // GET: Location/Wards/271  (271 là id quận/huyện)
        public async Task<ActionResult> Wards(int id)
        {
            try
            {
                string apiUrl = $"https://vapi.vnappmob.com/api/v2/province/ward/{id}";
                var response = await client.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return Content(content, "application/json");
            }
            catch
            {
                return new HttpStatusCodeResult(500, "Lỗi khi lấy dữ liệu Phường/Xã");
            }
        }
    }
}

