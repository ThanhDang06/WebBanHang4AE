using System.Web.Mvc;
using System.Web.Routing;

namespace WBH
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route tường minh cho Search
            routes.MapRoute(
                name: "ProductsSearch",
                url: "Products/Search",
                defaults: new { controller = "Products", action = "Search" }
            );

            routes.MapRoute(
               name: "Default",
               url: "{controller}/{action}/{id}",
               defaults: new { controller = "Products", action = "ProductList", id = UrlParameter.Optional }
           );


            // CatchAll phải để cuối cùng
            routes.MapRoute(
                name: "CatchAll",
                url: "{*url}",
                defaults: new { controller = "WBH", action = "Home" },
                constraints: new { url = @"^(?!Products/Search|Carts|Sales).*" } // bỏ qua Products/Search/Sales
            );
        }
    }
}
