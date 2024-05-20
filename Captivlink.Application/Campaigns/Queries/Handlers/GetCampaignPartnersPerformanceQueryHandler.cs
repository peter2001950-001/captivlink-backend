using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Campaigns.Utility;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCampaignPartnersPerformanceQueryHandler : IQueryHandler<GetCampaignPartnersPerformanceQuery, PaginatedResult<CampaignPartnerPerformanceResult>?>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IMapper _mapper;
        public GetCampaignPartnersPerformanceQueryHandler(IUserRepository userRepository, ICampaignRepository campaignRepository, ICampaignEventRepository campaignEventRepository, ICampaignPartnerRepository campaignPartnerRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
            _campaignEventRepository = campaignEventRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CampaignPartnerPerformanceResult>?> Handle(GetCampaignPartnersPerformanceQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var paginatedOptions = _mapper.Map<PaginationOptions>(request);

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Id == request.CampaignId && x.Company.Id == user.Company.Id);
            if (campaign == null) return null;

            var partners = await _campaignPartnerRepository.FindWhereAsync(x => x.Campaign.Id == campaign.Id && x.Status == CampaignPartnerStatus.Approved, paginatedOptions);
            var partnersCount = await _campaignPartnerRepository.CountWhereAsync(x =>
                x.Campaign.Id == campaign.Id && x.Status == CampaignPartnerStatus.Approved);

            var events = (await _campaignEventRepository.FindWhereAsync(x => x.CampaignPartner.Campaign.Id == campaign.Id, null)).ToList();
            var eventsForPeriod = events.Where(x => x.CreatedOn > request.StartDate && x.CreatedOn < request.EndDate).ToList();
            var awards = campaign.Awards.ToList();

            var result = GenerateCampaignCreatorPerformanceResults(eventsForPeriod, events, partners.Select(x=>x.ContentCreator).ToList(), awards, campaign.BudgetPerCreator);

            return new PaginatedResult<CampaignPartnerPerformanceResult>(paginatedOptions, partnersCount, result);
        }

        private List<CampaignPartnerPerformanceResult> GenerateCampaignCreatorPerformanceResults(List<CampaignEvent> eventsForPeriod, List<CampaignEvent> allEvents, List<PersonDetails> partners, List<Award> awards, decimal budget)
        {
            var result = new List<CampaignPartnerPerformanceResult>();
            foreach (var partner in partners)
            {
                var periodEventsForPartner = eventsForPeriod.Where(x => x.CampaignPartner.ContentCreator.Id == partner.Id).ToList();
                var allEventForPartner = allEvents.Where(x => x.CampaignPartner.ContentCreator.Id == partner.Id).ToList();

                result.Add(new CampaignPartnerPerformanceResult()
                {
                    ContentCreator = _mapper.Map<PersonResult>(partner),
                    MetricsForPeriod = CampaignPerformanceUtils.GenerateMetrics(awards, periodEventsForPartner),
                    MetricsLifetime = CampaignPerformanceUtils.GenerateMetrics(awards, allEventForPartner),
                    Budget = budget
                });
            }

            return result;
        }
    }
}
