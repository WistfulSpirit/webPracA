using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(webPracA.Startup))]
namespace webPracA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
