using DfQuantycaDocuments.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Tracing;

namespace DfQuantycaDocuments
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            
            /*
            config.EnableSystemDiagnosticsTracing();
            if (config == null)
            {
                throw new ArgumentNullException("config",
                    @"Expected type HttpConfiguration.");

            }
            */
            log4net.Config.XmlConfigurator.Configure();
            config.Services.Replace(typeof(ITraceWriter), new DfQuantycaTracert());


            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
