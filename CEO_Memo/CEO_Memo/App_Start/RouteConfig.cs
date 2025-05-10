using System.Web.Mvc;
using System.Web.Routing;

public class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

        // Định nghĩa route mặc định
        routes.MapRoute(
            name: "Default",
            url: "{controller}/{action}/{id}",
            defaults: new { controller = "Home", action = "Dashboard", id = UrlParameter.Optional }
        );

        // Định nghĩa route cho đăng nhập (để người dùng có thể điều hướng đến trang đăng nhập khi chưa đăng nhập)
        routes.MapRoute(
            name: "Login",
            url: "Auth/Login",
            defaults: new { controller = "Auth", action = "Login" }
        );
    }
}
