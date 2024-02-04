using Captivlink.Api.Data;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Api.Utility
{
    public static class Seeder
    {
        public static async Task RunDbMigrations(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            using var scope = provider.GetService<IServiceScopeFactory>().CreateScope();

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await applicationDbContext.Database.MigrateAsync();
        }

        public static async Task SeedDatabase(IServiceCollection services)
        {
            var provider = services.BuildServiceProvider();
            await SeedDatabase(provider);
        }

        public static async Task SeedDatabase(IServiceProvider provider)
        {
            using var scope = provider.GetService<IServiceScopeFactory>().CreateScope();

            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await scope.ServiceProvider.GetRequiredService<AzurePersistedGrantDbContext>().Database.MigrateAsync();

            var configurationContext = scope.ServiceProvider.GetRequiredService<AzureConfigurationDbContext>();
            await configurationContext.Database.MigrateAsync();

            if (!Program.Application.Switches.SeedData)
                return;

            var environment = Program.Application.Environment;
            if (string.IsNullOrWhiteSpace(environment))
                environment = "Local";

            SeedIdentityResources(configurationContext, environment);
            SeedRoles(applicationDbContext);


            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            if (!userManager.Users.Any(x => x.UserName == "creator"))
            {
                var user = new ApplicationUser()
                {
                    Email = "test1@admin.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    UserName = "creator"
                };
                var response = await userManager.CreateAsync(user, "Sample123!");
                   
                await userManager.AddToRoleAsync(user, "CONTENTCREATOR");
            }
        }

        private static void SeedRoles(ApplicationDbContext context)
        {
            context.Roles.RemoveRange(context.Roles.ToList());
            foreach (var role in Config.GetRoles())
            {
                context.Roles.Add(role);
            }

            context.SaveChanges();
        }

       

        private static void SeedIdentityResources(AzureConfigurationDbContext context, string environment)
        {
            context.Clients.RemoveRange(context.Clients.Where(x => !x.RequireClientSecret).ToList());
            context.IdentityResources.RemoveRange(context.IdentityResources.ToList());
            context.ApiResources.RemoveRange(context.ApiResources.ToList());
            context.ApiScopes.RemoveRange(context.ApiScopes.ToList());

            foreach (var client in Config.GetClientsForEnvironment(environment))
            {
                context.Clients.Add(client.ToEntity());
            }

            context.SaveChanges();

            foreach (var resource in Config.IdentityResources)
            {
                context.IdentityResources.Add(resource.ToEntity());
            }

            context.SaveChanges();

            foreach (var resource in Config.GetApis())
            {
                context.ApiResources.Add(resource.ToEntity());
            }

            foreach (var scopes in Config.ApiScopes)
            {
                context.ApiScopes.Add(scopes.ToEntity());
            }

            context.SaveChanges();
        }

    }
}
