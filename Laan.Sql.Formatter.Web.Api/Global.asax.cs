using System;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Laan.Sql.Formatter.Web.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration
                .Configure(WebApiConfig.Register);

            GlobalConfiguration
                .Configuration
                .Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
        }
    }
}
