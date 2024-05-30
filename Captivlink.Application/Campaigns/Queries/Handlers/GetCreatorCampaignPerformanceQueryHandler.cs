using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Campaigns.Utility;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCreatorCampaignPerformanceQueryHandler : IQueryHandler<GetCreatorCampaignPerformanceQuery, CreatorCampaignPerformanceResult?>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;

        public GetCreatorCampaignPerformanceQueryHandler(IUserRepository userRepository, ICampaignEventRepository campaignEventRepository, ICampaignPartnerRepository campaignPartnerRepository, ICampaignRepository campaignRepository)
        {
            _userRepository = userRepository;
            _campaignEventRepository = campaignEventRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<CreatorCampaignPerformanceResult?> Handle(GetCreatorCampaignPerformanceQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Person == null)
            {
                return null;
            }

            var campaignPartner = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x => x.Campaign.Id == request.CampaignId && x.ContentCreator.Id == user.Person.Id);
            if (campaignPartner == null) return null;

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Id == campaignPartner.Campaign.Id);

            var events = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Id == campaignPartner.Id, null)).ToList();
            var eventsForPeriod = events.Where(x => x.CreatedOn > request.StartDate && x.CreatedOn < request.EndDate).ToList();
            var awards = campaign!.Awards.ToList();

            var result = new CreatorCampaignPerformanceResult()
            {
                MetricsForPeriod = CampaignPerformanceUtils.GenerateMetrics(awards, eventsForPeriod),
                MetricsLifetime = CampaignPerformanceUtils.GenerateMetrics(awards, events),
                ClickData = CampaignPerformanceUtils.GenerateClickData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseData = CampaignPerformanceUtils.GeneratePurchaseData(eventsForPeriod, request.StartDate, request.EndDate),
                PurchaseValueData = CampaignPerformanceUtils.GeneratePurchaseValueData(eventsForPeriod, request.StartDate, request.EndDate)
            };
            return result;
        }
    }
}
