using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(mindblog.Startup))]
namespace mindblog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
