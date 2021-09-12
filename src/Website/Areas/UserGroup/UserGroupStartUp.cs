using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Headlight.Areas.User.UserHostingStartup))]
namespace Headlight.Areas.UserGroup
{
    public class UserGroupStartUp : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}