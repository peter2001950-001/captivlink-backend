using Captivlink.Backend.Utility;

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
