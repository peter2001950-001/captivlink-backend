using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
    {
        public CampaignRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<Campaign> Query => DbContext.Campaigns.Include(x => x.Categories).Include(x => x.Website).Include(x => x.Awards);
    }
}
