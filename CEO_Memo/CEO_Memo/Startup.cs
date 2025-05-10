using Microsoft.Owin.Security.Jwt;
using Microsoft.Owin.Security;
using Microsoft.Owin;
using Owin;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;

[assembly: OwinStartup(typeof(CEO_Memo.Startup))]

namespace CEO_Memo
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Các cấu hình middleware khác có thể ở đây
        }
    }
}
