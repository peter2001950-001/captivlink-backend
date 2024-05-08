using Captivlink.Infrastructure.Configuration;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new PersonDetailsConfiguration());
            builder.ApplyConfiguration(new CompanyDetailsConfiguration());
            builder.ApplyConfiguration(new CampaignConfiguration());
            builder.Entity<ApplicationUser>().HasOne(x => x.Person);
            builder.Entity<ApplicationUser>().HasOne(x => x.Company);

            builder.Entity<Campaign>().HasOne(x => x.Company).WithMany().IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<CampaignPartner>().HasOne(x => x.Campaign).WithMany().IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.Entity<CampaignPartner>().HasOne(x => x.ContentCreator).WithMany().IsRequired()
                .OnDelete(DeleteBehavior.ClientSetNull);

            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Website> Websites { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignPartner> CampaignPartners { get; set; }
    }
}
