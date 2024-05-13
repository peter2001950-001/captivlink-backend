using Captivlink.Backend.Services;
using Captivlink.Backend.Services.Contract;

namespace Captivlink.Backend
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
