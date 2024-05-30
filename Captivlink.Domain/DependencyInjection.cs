using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Events.Providers;
using Captivlink.Infrastructure.Repositories;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Captivlink.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IConfiguration>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, ApplicationDbContext>();
            services.AddScoped<IWebsiteRepository, WebsiteRepository>();
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<ICampaignPartnerRepository, CampaignPartnerRepository>();
            services.AddScoped<IProducerProvider, ProducerProvider>();
            services.AddScoped<ICacheService, CacheService>();
            services.AddScoped<ICampaignEventRepository, CampaignEventRepository>();
            services.AddScoped<IPerformanceCacheService, PerformanceCacheService>();

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetConnectionString("Redis");
            });

            EncryptionKey = config?["EncryptionKey"];

            return services;
        }
        public static string? EncryptionKey { get; private set;  }
    }
}
