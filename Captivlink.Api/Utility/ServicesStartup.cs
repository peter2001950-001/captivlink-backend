using Captivlink.Api.Utility;
using Captivlink.Api.Utility.CategorySeeder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(ServicesStartup))]
namespace Captivlink.Api.Utility
{

    public class ServicesStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                services.AddTransient<ICategorySeeder, CategorySeeder.CategorySeeder>();
            });
        }
    }
}
