using Microsoft.Owin;

[assembly: OwinStartup(typeof(SqlWebApi.Startup))]

namespace SqlWebApi
{
    using Owin;
    using System.Diagnostics;
    using System.Web.Http;

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=316888

            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);

            app.Use<LoggerMiddleware>();
        }
    }
}
