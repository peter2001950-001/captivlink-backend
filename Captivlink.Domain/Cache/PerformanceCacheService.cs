using Captivlink.Infrastructure.Cache.Models;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Infrastructure.Cache
{
    public class PerformanceCacheService : IPerformanceCacheService
    {
        private readonly ICacheService _cacheService;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;

        public PerformanceCacheService(ICacheService cacheService, ICampaignRepository campaignRepository, ICampaignPartnerRepository campaignPartnerRepository, ICampaignEventRepository campaignEventRepository)
        {
            _cacheService = cacheService;
            _campaignRepository = campaignRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignEventRepository = campaignEventRepository;
        }

        public async Task CampaignEventAdded(Guid campaignId)
        {
            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Id == campaignId);
            if (campaign == null) return;
            
            await _cacheService.DeleteAsync("campaign_performance_" + campaign.Id);
            await GetCampaignPerformanceAsync(campaignId);

            var partners = await _campaignPartnerRepository.FindWhereAsync(x => x.Campaign.Id == campaignId && x.Status == CampaignPartnerStatus.Approved, null);
            foreach (var partner in partners)
            {
                await _cacheService.DeleteAsync("campaign_creator_performance_" + partner.Id);
                await GetCreatorCampaignPerformance(partner.Id);
            }
        }
        public async Task<CampaignPerformance> GetCampaignPerformanceAsync(Guid campaignId)
        {
            var cached = await _cacheService.GetAsync<CampaignPerformance>("campaign_performance_" + campaignId);
            if (cached != null) return cached;

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Id == campaignId);
            if (campaign == null) return new CampaignPerformance();

            var campaignPerformance = await GenerateCampaignPerformanceAsync(campaign);
            await _cacheService.SetAsync("campaign_performance_" + campaignId, campaignPerformance);

            return campaignPerformance;
        }

        public async Task<CreatorCampaignPerformance> GetCreatorCampaignPerformance(Guid campaignPartnerId)
        {
            var cached = await _cacheService.GetAsync<CreatorCampaignPerformance>("campaign_creator_performance_" + campaignPartnerId);
            if (cached != null) return cached;

            var campaignPartner = await _campaignPartnerRepository.GetCampaignPartnerByIdAsync(campaignPartnerId);
            if(campaignPartner == null) return new CreatorCampaignPerformance();

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Id == campaignPartner.Campaign.Id);
            if (campaign == null) return new CreatorCampaignPerformance();

            var campaignPerformance = await GenerateCreatorCampaignPerformanceAsync(campaignPartner, campaign.Awards.ToList());
            await _cacheService.SetAsync("campaign_creator_performance_" + campaignPartnerId, campaignPerformance);

            return campaignPerformance;
        }

        private async Task<CampaignPerformance> GenerateCampaignPerformanceAsync(Campaign campaign)
        {
            var allEvents = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Campaign.Id == campaign.Id, null)).ToList();
            return new CampaignPerformance()
            {
                ClicksCount = allEvents.Count(x => x.Type == CampaignEventType.Click),
                PurchasesCount = allEvents.Count(x => x.Type == CampaignEventType.Purchase),
                TotalValue = (decimal)allEvents.Sum(x => x.Amount ?? 0)
            };
        }

        private async Task<CreatorCampaignPerformance> GenerateCreatorCampaignPerformanceAsync(CampaignPartner campaignPartner, List<Award> awards)
        {
            var allEvents = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Id == campaignPartner.Id, null)).ToList();
            
            var performance = new CreatorCampaignPerformance()
            {
                ClicksCount = allEvents.Count(x => x.Type == CampaignEventType.Click),
                PurchasesCount = allEvents.Count(x => x.Type == CampaignEventType.Purchase),
                TotalValue = (decimal)allEvents.Sum(x => x.Amount ?? 0),
            };

            performance.TotalEarned = CalculateSpentAmount(performance.ClicksCount, performance.PurchasesCount,
                performance.TotalValue, awards);

            if(performance.TotalEarned > campaignPartner.Campaign.BudgetPerCreator)
                performance.TotalEarned = campaignPartner.Campaign.BudgetPerCreator;

            return performance;
        }

        private decimal CalculateSpentAmount(int totalClicks, int totalPurchase, decimal totalValue, List<Award> awards)
        {
            var perClickReward =
                awards.FirstOrDefault(x => x.Type == AwardType.PerClick)?.Amount ?? 0;
            var perPurchaseReward =
                awards.FirstOrDefault(x => x.Type == AwardType.PerConversion)?.Amount ?? 0;
            var percentageReward =
                awards.FirstOrDefault(x => x.Type == AwardType.Percentage)?.Amount ?? 0;

            var totalSpent = totalClicks * perClickReward +
                             totalPurchase * perPurchaseReward +
                             totalValue * (percentageReward / 100);

            return totalSpent;
        }
    }
}
