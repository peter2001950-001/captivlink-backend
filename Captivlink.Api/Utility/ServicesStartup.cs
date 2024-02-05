using Captivlink.Backend.Utility;
using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(ServicesStartup))]
namespace Captivlink.Backend.Utility
{

    public class ServicesStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) =>
            {
                //services.AddTransient<IAccountService, AccountService>()
            });
        }
    }
}
