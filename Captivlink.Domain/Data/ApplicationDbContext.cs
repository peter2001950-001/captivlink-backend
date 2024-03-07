﻿using Captivlink.Infrastructure.Configuration;
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
            builder.Entity<ApplicationUser>().HasOne(x => x.Person);
            builder.Entity<ApplicationUser>().HasOne(x => x.Company);
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public async Task SaveChangesAsync()
        {
            await base.SaveChangesAsync();
        }
    }
}
