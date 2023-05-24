using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Everyday.Startup))]
namespace Everyday
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
