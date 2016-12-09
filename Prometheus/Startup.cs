using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Domino.Startup))]
namespace Domino
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            
        }
    }
}
