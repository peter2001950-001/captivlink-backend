using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Repositories;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;
using Microsoft.Extensions.DependencyInjection;

namespace Captivlink.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            return services;
        }
    }
}
