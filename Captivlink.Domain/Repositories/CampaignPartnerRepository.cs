using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignPartnerRepository : BaseRepository<CampaignPartner>, ICampaignPartnerRepository
    {
        private readonly ICacheService _cacheService;
        public CampaignPartnerRepository(ApplicationDbContext dbContext, ICacheService cacheService) : base(dbContext)
        {
            _cacheService = cacheService;
        }

        protected override IQueryable<CampaignPartner> Query => DbContext.CampaignPartners.Include(x => x.Campaign).Include(x => x.ContentCreator).AsQueryable();

        public async Task<PaginatedResult<CampaignPartner>> GetPagedCreatorCampaignsAsync(Guid creatorId, PaginationOptions paginationOptions, CampaignPartnerStatus? status)
        {
            IQueryable<CampaignPartner> query = Query.Where(x=>x.ContentCreator.Id == creatorId && !x.IsDeleted).Include(x => x.Campaign).ThenInclude(x => x.Company).Include(x => x.Campaign)
                .ThenInclude(x => x.Awards).Include(x => x.Campaign).ThenInclude(x => x.Categories);

            if (status.HasValue)
            {
                query = query.Where(x=>x.Status == status.Value).AsQueryable();
            }

            var items = await query.Sorted(paginationOptions).Paged(paginationOptions).ToListAsync();
            var totalCount = await query.CountAsync();

            return new PaginatedResult<CampaignPartner>(paginationOptions, totalCount, items);

        }

        public async Task<CampaignPartner?> GetCampaignPartnerByAffCodeAsync(string affiliateCode)
        {
            var cachedResult = await _cacheService.GetAsync<CampaignPartner>(affiliateCode);
            if (cachedResult != null) return cachedResult;
            
            var entity = await Query.FirstOrDefaultAsync(x => x.AffiliateCode == affiliateCode);
            if (entity == null)
                return null;

            await _cacheService.SetAsync(affiliateCode, entity);
            return entity;
        }

        public async Task<CampaignPartner?> GetCampaignPartnerByIdAsync(Guid campaignCreatorId)
        {
            var cachedResult = await _cacheService.GetAsync<CampaignPartner>("campaign_partner_id" + campaignCreatorId);
            if (cachedResult != null) return cachedResult;

            var entity = await Query.Include(x => x.Campaign.Website).FirstOrDefaultAsync(x => x.Id == campaignCreatorId);
            if (entity == null)
                return null;

            await _cacheService.SetAsync("campaign_partner_id" + campaignCreatorId, entity);
            return entity;
        }

        public async Task ClearCampaignPartnerCache(Guid campaignId)
        {
            var partners = await Query.Where(x => x.Campaign.Id == campaignId).ToListAsync();
            foreach (var partner in partners)
            {
               await _cacheService.DeleteAsync("campaign_partner_id" + partner.Id);
            }
        }
    }
}
