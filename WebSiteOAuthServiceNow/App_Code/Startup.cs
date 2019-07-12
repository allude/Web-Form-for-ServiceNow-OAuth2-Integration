using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSiteOAuthServiceNow.Startup))]
namespace WebSiteOAuthServiceNow
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
