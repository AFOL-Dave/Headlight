using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Headlight.Areas.User.UserHostingStartup))]
namespace Headlight.Areas.User
{
    public class UserHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}