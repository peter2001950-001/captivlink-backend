using Captivlink.PublicApi.Utility;

[assembly: HostingStartup(typeof(ServicesStartup))]
namespace Captivlink.PublicApi.Utility
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
