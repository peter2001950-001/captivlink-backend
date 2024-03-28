using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class WebsiteRepository : BaseRepository<Website>,  IWebsiteRepository
    {
        public WebsiteRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<Website> Query => DbContext.Websites;

        protected override Task<Website> BeforeAddAsync(Website entity)
        {
            entity.AccessToken = RandomString(64);
            return base.BeforeAddAsync(entity);
        }

        public async Task<bool> IsWebsiteExisting(Website website)
        {
            return await Query.AnyAsync(x => (x.Domain == website.Domain || x.Name == website.Name) && x.Company.Id == website.Company.Id && x.Id != website.Id);
        }

        protected override async Task<bool> CanAddAsync(Website entity)
        {
            return !await IsWebsiteExisting(entity);
        }

        public static string RandomString(int length)
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
