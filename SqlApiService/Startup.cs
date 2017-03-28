using Microsoft.Owin;

[assembly: OwinStartup(typeof(SqlApiService.Startup))]

namespace SqlApiService
{
    using Owin;
    using LightNode.Server;
    using LightNode.Formatter;
    using LightNode.Swagger;
    using System;
    using Microsoft.Owin.Hosting;

    /// <summary>
    /// 開始
    /// </summary>
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
        /// <summary>
        /// 設定
        /// </summary>
        /// <param name="app"></param>
        public void Configuration(IAppBuilder app)
        {
            app.UseRequestScopeContext();
            app.UseFileServer();

            app.Map("/api", builder =>
            {
                var option = new LightNodeOptions(AcceptVerbs.Get | AcceptVerbs.Post, new JsonNetContentFormatter(), new JsonNetContentFormatter())
                {
                    ParameterEnumAllowsFieldNameParse = true,
                    ErrorHandlingPolicy = ErrorHandlingPolicy.ReturnInternalServerErrorIncludeErrorDetails,
                    OperationMissingHandlingPolicy = OperationMissingHandlingPolicy.ReturnErrorStatusCodeIncludeErrorDetails,
                };

                builder.UseLightNode(option);
            });

            app.Map("/pages", builder =>
            {
                builder.UseLightNode(new LightNodeOptions(AcceptVerbs.Get, new JsonNetContentFormatter()));
            });

            app.Map("/swagger", builder =>
            {
                var xmlName = "SqlApiService.xml";
                var xmlPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\" + xmlName;

                builder.UseLightNodeSwagger(new SwaggerOptions("SqlApiService", "/api")
                {
                    XmlDocumentPath = xmlPath,
                    IsEmitEnumAsString = true
                });
            });
        }
    }
}
