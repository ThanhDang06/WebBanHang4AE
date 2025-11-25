using System;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace WBH
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            AttachDatabaseIfNotExists();
        }
        private void AttachDatabaseIfNotExists()
        {
            // Đường dẫn tới file .mdf trong App_Data
            var dbFile = Server.MapPath("~/App_Data/DBFashionStore.mdf");
            var connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True;";

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $@"
                        IF NOT EXISTS(SELECT name FROM sys.databases WHERE name = N'DBFashionStore')
                        BEGIN
                            CREATE DATABASE DBFashionStore
                            ON (FILENAME = '{dbFile}')
                            FOR ATTACH_REBUILD_LOG;
                        END";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Context.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                if (authTicket != null)
                {
                    string[] roles = authTicket.UserData.Split(',');
                    System.Security.Principal.GenericPrincipal userPrincipal =
                        new System.Security.Principal.GenericPrincipal(
                            new System.Security.Principal.GenericIdentity(authTicket.Name),
                            roles);
                    Context.User = userPrincipal;
                }
            }
        }
    }
}
