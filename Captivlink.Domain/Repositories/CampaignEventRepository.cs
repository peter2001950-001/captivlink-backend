using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Cache.Models;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignEventRepository : BaseRepository<CampaignEvent>, ICampaignEventRepository
    {
        private readonly ICacheService _cacheService;
        public CampaignEventRepository(ApplicationDbContext dbContext, ICacheService cacheService) : base(dbContext)
        {
            _cacheService = cacheService;
        }

        protected override IQueryable<CampaignEvent> Query => DbContext.CampaignEvents.AsQueryable();

        protected override async Task<CampaignEvent> AfterAddAsync(CampaignEvent entity)
        {
            var campaignPerformance = await GenerateCampaignPerformanceAsync(entity.CampaignPartner.Campaign.Id);
            await _cacheService.SetAsync("campaign_performance_" + entity.CampaignPartner.Campaign.Id, campaignPerformance);

            return await base.AfterAddAsync(entity);
        }

        public async Task<CampaignPerformance> GetCampaignPerformanceAsync(Guid campaignId)
        {
            var cached = await _cacheService.GetAsync<CampaignPerformance>("campaign_performance_" + campaignId);
            if (cached != null) return cached;
            
            var campaignPerformance = await GenerateCampaignPerformanceAsync(campaignId);
            await _cacheService.SetAsync("campaign_performance_" + campaignId, campaignPerformance);

            return campaignPerformance;

        }

        private async Task<CampaignPerformance> GenerateCampaignPerformanceAsync(Guid campaignId)
        {
            var allEvents = await Query.Where(x => x.CampaignPartner.Campaign.Id == campaignId).ToListAsync();
            return new CampaignPerformance()
            {
                ClicksCount = allEvents.Count(x => x.Type == CampaignEventType.Click),
                PurchasesCount = allEvents.Count(x => x.Type == CampaignEventType.Purchase),
                TotalValue = (decimal) allEvents.Sum(x => x.Amount ?? 0)
            };

        }
    }
}
