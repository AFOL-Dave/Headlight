using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Headlight.Areas.LugBulk.LugBulkHostingStartup))]
namespace Headlight.Areas.LugBulk
{
    public class LugBulkHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => { });
        }
    }
}