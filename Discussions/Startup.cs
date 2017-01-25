using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Discussions.Startup))]
namespace Discussions
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
