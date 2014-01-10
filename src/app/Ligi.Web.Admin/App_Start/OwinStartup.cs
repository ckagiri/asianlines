using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Ligi.Web.Admin.Startup))]

namespace Ligi.Web.Admin
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}