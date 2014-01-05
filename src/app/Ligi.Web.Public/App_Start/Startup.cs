using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Ligi.Web.Public.Startup))]

namespace Ligi.Web.Public
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}