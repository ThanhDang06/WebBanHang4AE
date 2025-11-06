using System.Web.Mvc;
using WBH.Filters;

namespace WBH
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthorizeUserAttribute());
            filters.Add(new BreadCrumbAttribute());
        }
    }
}
