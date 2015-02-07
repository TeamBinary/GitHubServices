using System.Web.Http;

namespace GitHubServices
{
  public static class WebApiConfig
  {
      public static void Register(HttpConfiguration config)
      {
          // Web API configuration and services

          // Web API routes
          config.MapHttpAttributeRoutes();

          config.Routes.MapHttpRoute(
              name: "DefaultApi",
              routeTemplate: "api/{controller}/{url}",
              defaults: new { url = RouteParameter.Optional });

          config.Routes.MapHttpRoute(
              name: "P1P2",
              routeTemplate: "api/test/{p1}/{p2}",
              defaults: new { controller = "Test" });


      }
  }
}
