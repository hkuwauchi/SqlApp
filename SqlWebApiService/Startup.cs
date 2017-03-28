using Microsoft.Owin;

[assembly: OwinStartup(typeof(SqlWebApiService.Startup))]

namespace SqlWebApiService
{
    using Microsoft.Owin.Hosting;
    using Owin;
    using SqlWebApi;
    using System;
    using System.Web.Http;

    public class Startup
    {
        /// <summary>
        /// サーバー名
        /// </summary>
        public string ServiceName { get; set; }
        private static IDisposable _application;
        /// <summary>
        /// 初期化
        /// </summary>
        /// <param name="serviceName"></param>
        public Startup(string serviceName)
        {
            ServiceName = serviceName;
        }
        /// <summary>
        /// TopShelfからの開始用
        /// </summary>
        public void Start()
        {
            string uri = string.Format("http://localhost:5050/");
            _application = WebApp.Start<Startup>(uri); ;
        }

        /// <summary>
        /// TopShelfからの停止用
        /// </summary>
        public void Stop()
        {
            _application?.Dispose();
        }

        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            WebApiConfig.Register(config);
            app.UseWebApi(config);
        }
    }
}
