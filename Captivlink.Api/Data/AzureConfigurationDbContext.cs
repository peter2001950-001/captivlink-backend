using Captivlink.Infrastructure.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Api.Data
{
    public class AzureConfigurationDbContext : ConfigurationDbContext<AzureConfigurationDbContext>
    {
        public AzureConfigurationDbContext(DbContextOptions<AzureConfigurationDbContext> options,
            ConfigurationStoreOptions storeOptions) : base(options, storeOptions)
        {
            this.ApplyAzureManagedIdentityToken(Program.Application.Switches.UseAccessToken);
        }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
    
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var table = entityType.GetTableName();
                if (!table.StartsWith("Identity_"))
                {
                    entityType.SetTableName("Identity_" + table);
                }
            }
        }
    }
}