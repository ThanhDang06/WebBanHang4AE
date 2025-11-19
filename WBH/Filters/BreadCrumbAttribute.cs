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
            var urlHelper = new UrlHelper(filterContext.RequestContext);

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
            else if (controller == "Admin" || controller == "Sales"|| controller == "AdminVouchers")
            {
                if (controller == "Admin" && action.Equals("Dashboard", StringComparison.OrdinalIgnoreCase))
                {
                    filterContext.Controller.ViewBag.Breadcrumb = null;
                    base.OnActionExecuting(filterContext);
                    return;
                }
                breadcrumb.Add(("Admin", urlHelper.Action("Dashboard", "Admin")));

                if (controller == "Admin")
                {
                    switch (action)
                    {
                        case "ProductManagement":
                            breadcrumb.Add(("Quản lý sản phẩm", null));
                            break;

                        case "Orders":
                            breadcrumb.Add(("Quản lý đơn hàng", null));
                            break;

                        case "CustomerManagement":
                            breadcrumb.Add(("Quản lý khách hàng", null));
                            break;

                        case "EditProduct":
                            breadcrumb.Add(("Quản lý sản phẩm", urlHelper.Action("ProductManagement", "Admin")));
                            breadcrumb.Add(("Chỉnh sửa sản phẩm", null));
                            break;

                        default:
                            breadcrumb.Add((action, null));
                            break;
                    }
                }
                if (controller == "Sales")
                {
                    breadcrumb.Add(("Quản lý khuyến mãi", null));
                }

                if (controller == "AdminVouchers")
                {
                    breadcrumb.Add(("Quản lý voucher", null));
                }   
            }

            // Gán vào ViewBag
            filterContext.Controller.ViewBag.Breadcrumb = breadcrumb;

            base.OnActionExecuting(filterContext);
        }
    }
}
