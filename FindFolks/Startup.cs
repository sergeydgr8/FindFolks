using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FindFolks.Startup))]
namespace FindFolks
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
