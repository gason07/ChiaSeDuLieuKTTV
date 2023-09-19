using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Net.Http.Formatting;

namespace CHIASEDULIEU
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
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Cấu hình hiển thị kiểu json / Tắt cấu hình này thì sẽ về mặc định xml
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new RequestHeaderMapping("Accept",
                "text/html",
                StringComparison.InvariantCultureIgnoreCase,
                true,
                "application/json"));
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings
     .ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            GlobalConfiguration.Configuration.Formatters
                .Remove(GlobalConfiguration.Configuration.Formatters.XmlFormatter);
        }
    }
}
