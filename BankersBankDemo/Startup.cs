using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BankersBankDemo.Startup))]
namespace BankersBankDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
