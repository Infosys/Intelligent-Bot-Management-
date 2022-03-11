using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Configuration;

namespace Infosys.Solutions.Ainauto.ConfigurationManagement.API
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            //config.MapHttpAttributeRoutes();
           
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            config.Formatters.JsonFormatter.SupportedMediaTypes
                .Add(new MediaTypeHeaderValue("text/html"));

            string host = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["URlforCORS"]);
            var corsAttr = new EnableCorsAttribute(host, "*", " * ");
            config.EnableCors(corsAttr);
        }
    }
}
