using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Ligi.Web.Admin
{
    public class RouteConfig
    {
        public static string ControllerOnly = "ApiControllerOnly";
        public static string ControllerAndId = "ApiControllerAndGuidId";
        public static string ControllerAction = "ApiControllerAction";

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: ControllerOnly,
                routeTemplate: "api/{controller}"
            );

            routes.MapHttpRoute(
                name: ControllerAndId,
                routeTemplate: "api/{controller}/{id}",
                defaults: null, //defaults: new { id = RouteParameter.Optional } //,
                constraints: new
                {
                    id = @"^(\{){0,1}[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9‌​a-fA-F]{12}(\}){0,1}$"
                }
            );

            routes.MapHttpRoute(
                name: ControllerAction,
                routeTemplate: "api/{controller}/{action}"
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}