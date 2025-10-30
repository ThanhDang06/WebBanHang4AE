using System.Web.Mvc;

namespace WBH.Filters
{
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            if (session["User"] == null)
            {
                // Chuyển hướng về trang Login nếu chưa đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                     new System.Web.Routing.RouteValueDictionary
                     {
                       { "controller", "Login" },
                       { "action", "DangNhap" }
                     }
                );
            }
        }
    }
}