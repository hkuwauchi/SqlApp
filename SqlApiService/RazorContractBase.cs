namespace SqlApiService
{
    using System;
    using System.IO;
    using LightNode.Server;
    using RazorEngine.Configuration;
    using RazorEngine.Templating;

    /// <summary>
    /// 
    /// </summary>
    public abstract class RazorContractBase : LightNodeContract
    {
        static readonly IRazorEngineService razor = CreateRazorEngineService();

        static IRazorEngineService CreateRazorEngineService()
        {
            var config = new TemplateServiceConfiguration()
            {
                DisableTempFileLocking = true,
                CachingProvider = new DefaultCachingProvider(_ => { }),
                TemplateManager = new DelegateTemplateManager(name =>
                {
                // import from "Views" directory
                var viewPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", name);
                    return File.ReadAllText(viewPath);
                })
            };
            return RazorEngineService.Create(config);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected string View(string viewName)
        {
            return View(viewName, new object());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewName"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        protected string View(string viewName, object model)
        {
            var type = model.GetType();
            if (razor.IsTemplateCached(viewName, type))
            {
                return razor.Run(viewName, type, model);
            }
            else
            {
                return razor.RunCompile(viewName, type, model);
            }
        }
    }
}
