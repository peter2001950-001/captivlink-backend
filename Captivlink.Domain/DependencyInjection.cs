using Captivlink.Infrastructure.Repositories;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Captivlink.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            return services;
        }
    }
}
