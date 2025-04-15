using System.Web.Http;

namespace CEO_Memo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Cho phép sử dụng [Route()] trên controller
            config.MapHttpAttributeRoutes();

            // Định nghĩa route dạng: /api/controller/action
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
