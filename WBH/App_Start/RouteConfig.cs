using System.Web.Mvc;
using System.Web.Routing;

namespace WBH
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "DangNhap", id = UrlParameter.Optional }

            );
            // Route bắt tất cả -> về Home/Index
            routes.MapRoute(
                name: "CatchAll",
                url: "{*url}",
                defaults: new { controller = "WBH", action = "Home" }
            );
        }
    }
}
