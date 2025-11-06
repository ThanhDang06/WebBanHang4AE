using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WBH.Models;

namespace WBH.Filters
{
    public class BreadCrumbAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            var action = filterContext.ActionDescriptor.ActionName;

            var db = new DBFashionStoreEntitiess();
            var breadcrumb = new List<(string Name, string Url)>();

            // Mapping category code → tên hiển thị
            var categoryNames = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "ao", "Áo" },
                { "quan", "Quần" },
                { "vo", "Vớ" },
                { "non", "Nón" },
                { "trangsuc", "Trang sức" }
            };

            // Luôn có Trang chủ
            breadcrumb.Add(("Trang chủ", new UrlHelper(filterContext.RequestContext).Action("ProductList", "Products")));

            if (controller == "Products")
            {
                if (action == "ClothesList")
                {
                    breadcrumb.Add(("Sản phẩm", null));
                }
                else if (action == "AccessoriesList")
                {
                    breadcrumb.Add(("Phụ kiện", null));
                }
                else if (action == "CategoryProducts")
                {
                    // Lấy category từ action parameters
                    if (filterContext.ActionParameters.ContainsKey("category"))
                    {
                        string category = filterContext.ActionParameters["category"] as string;
                        if (!string.IsNullOrEmpty(category))
                        {
                            string displayName = categoryNames.ContainsKey(category) ? categoryNames[category] : category;
                            // Xác định parent dựa trên category
                            string parent = (category == "ao" || category == "quan") ? "Sản phẩm" : "Phụ kiện";
                            string parentUrl = (category == "ao" || category == "quan") ?
                                new UrlHelper(filterContext.RequestContext).Action("ClothesList", "Products") :
                                new UrlHelper(filterContext.RequestContext).Action("AccessoriesList", "Products");

                            breadcrumb.Add((parent, parentUrl));
                            breadcrumb.Add((displayName, null));
                        }
                    }
                }
                else if (action == "Details")
                {
                    if (filterContext.ActionParameters.ContainsKey("id"))
                    {
                        int productId = Convert.ToInt32(filterContext.ActionParameters["id"]);
                        var product = db.Products.FirstOrDefault(p => p.IDProduct == productId);
                        if (product != null)
                        {
                            string parent = (product.Category == "ao" || product.Category == "quan") ? "Sản phẩm" : "Phụ kiện";
                            string parentUrl = (product.Category == "ao" || product.Category == "quan") ?
                                new UrlHelper(filterContext.RequestContext).Action("ClothesList", "Products") :
                                new UrlHelper(filterContext.RequestContext).Action("AccessoriesList", "Products");

                            breadcrumb.Add((parent, parentUrl));
                            breadcrumb.Add((product.ProductName, null));
                        }
                    }
                }
            }


            // Gán vào ViewBag
            filterContext.Controller.ViewBag.Breadcrumb = breadcrumb;

            base.OnActionExecuting(filterContext);
        }
    }
}
