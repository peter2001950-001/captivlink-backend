using Captivlink.PublicApi.Services;
using Captivlink.PublicApi.Services.Contract;

namespace Captivlink.PublicApi
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IConfiguration>();

            services.AddScoped<ILinkService, LinkService>();

            return services;
        }
    }
}
