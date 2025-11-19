using System.Web.Mvc;

namespace WBH.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;

            // Lấy thông tin controller/action hiện tại
            string controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
            string action = filterContext.ActionDescriptor.ActionName;

            if ((controller == "Products" &&
                        (action == "ProductList" || action == "Sale" || action == "ClothesList" || action == "AccessoriesList" || action == "Details"|| action == "Search" || action == "CategoryProducts" || action == "SearchAjax")) ||
                        (controller == "Login" && (action == "DangNhap" || action == "DangKy"))||
                         (controller == "Cart" && action == "Index" || action == "GetCartItems")

                        )
            {
                return; // không redirect
            }

            //Sửa đúng session key (vì bạn lưu là "UserName", không phải "User")
            if (session["UserName"] == null)
            {
                string returnUrl = filterContext.HttpContext.Request.RawUrl;

                filterContext.Controller.TempData["ReturnUrl"] = returnUrl;

                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "DangNhap" },
                        { "returnUrl", returnUrl }
                    }
                );
            }
        }
    }
}