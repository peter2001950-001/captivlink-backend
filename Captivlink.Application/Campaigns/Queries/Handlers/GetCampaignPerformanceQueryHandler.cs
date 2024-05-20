using AutoMapper;
using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Campaigns.Utility;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCampaignPerformanceQueryHandler: IQueryHandler<GetCampaignPerformanceQuery, CampaignPerformanceResult?>

    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;

        public GetCampaignPerformanceQueryHandler(IUserRepository userRepository, ICampaignEventRepository campaignEventRepository, ICampaignPartnerRepository campaignPartnerRepository, ICampaignRepository campaignRepository)
        {
            _userRepository = userRepository;
            _campaignEventRepository = campaignEventRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<CampaignPerformanceResult?> Handle(GetCampaignPerformanceQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var campaign = await  _campaignRepository.GetFirstOrDefaultAsync(x =>  x.Id == request.CampaignId && x.Company.Id == user.Company.Id);
            if (campaign == null) return null;

            var partners = await _campaignPartnerRepository.FindWhereAsync(x => x.Campaign.Id == campaign.Id && x.Status == CampaignPartnerStatus.Approved, null);
            var events = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Campaign.Id == campaign.Id, null)).ToList();
            var eventsForPeriod = events.Where(x => x.CreatedOn > request.StartDate && x.CreatedOn < request.EndDate).ToList();
            var awards = campaign.Awards.ToList();

            var result = new CampaignPerformanceResult()
            {
                MetricsForPeriod = CampaignPerformanceUtils.GenerateMetrics(awards, eventsForPeriod),
                MetricsLifetime = CampaignPerformanceUtils.GenerateMetrics(awards, events),
                TotalPartnerships = partners.Count(),
                ClickData = CampaignPerformanceUtils.GenerateClickData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseData = CampaignPerformanceUtils.GeneratePurchaseData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseValueData = CampaignPerformanceUtils.GeneratePurchaseValueData(eventsForPeriod, request.StartDate, request.EndDate)
            };
            return result;
        }

       
    }
}
