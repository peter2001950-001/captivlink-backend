using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignEventRepository : BaseRepository<CampaignEvent>, ICampaignEventRepository
    {
        public CampaignEventRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<CampaignEvent> Query => DbContext.CampaignEvents.AsQueryable();

    }
}
