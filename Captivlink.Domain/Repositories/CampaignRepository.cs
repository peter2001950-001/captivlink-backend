using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignRepository : BaseRepository<Campaign>, ICampaignRepository
    {

        private readonly ICacheService _cacheService;
        public CampaignRepository(ApplicationDbContext dbContext, ICacheService cacheService) : base(dbContext)
        {
            _cacheService = cacheService;
        }

        protected override IQueryable<Campaign> Query => DbContext.Campaigns.Include(x => x.Categories).Include(x => x.Website).Include(x => x.Awards).Include(x=>x.Company);

        protected override async Task<Campaign> BeforeUpdateAsync(Guid id, Campaign entity, Campaign localEntity)
        {
            if (entity.Status == CampaignStatus.Live && entity.Link != localEntity.Link)
            {
                var partners = await DbContext.CampaignPartners.Where(x => x.Campaign.Id == entity.Id).ToListAsync();
                foreach (var partner in partners)
                {
                    if(partner.AffiliateCode == null) break;

                    await _cacheService.SetAsync(partner.AffiliateCode, partner);
                }
            }
            return await base.BeforeUpdateAsync(id, entity, localEntity);
        }
    }
}
