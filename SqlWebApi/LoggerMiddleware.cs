namespace SqlWebApi
{
    using Microsoft.Owin;
    using System.Diagnostics;
    using System.Threading.Tasks;

    public class LoggerMiddleware : OwinMiddleware
    {
        public LoggerMiddleware(OwinMiddleware next)
        : base(next)
        {
        }

        public override async Task Invoke(IOwinContext context)
        {
            Debug.WriteLine("★ My Logger Start");
            await Next.Invoke(context);
            Debug.WriteLine("★ My Logger End");
        }
    }
}