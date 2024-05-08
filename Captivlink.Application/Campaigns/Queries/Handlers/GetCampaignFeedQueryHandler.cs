using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCampaignFeedQueryHandler : IQueryHandler<GetCampaignFeedQuery, PaginatedResult<CampaignCreatorResult>?>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;

        public GetCampaignFeedQueryHandler(ICampaignRepository campaignRepository, IUserRepository userRepository, IMapper mapper, ICampaignPartnerRepository campaignPartnerRepository)
        {
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _campaignPartnerRepository = campaignPartnerRepository;
        }

        public async Task<PaginatedResult<CampaignCreatorResult>?> Handle(GetCampaignFeedQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            if (user == null || user.Person == null)
            {
                return null;
            }

            var categories = user.Person.Categories.Select(x => x.Id);

            var result = await _campaignRepository.FindWhereAsync(
                x => x.Categories.Any(p => categories.Contains(p.Id)) && x.Status == CampaignStatus.Live,
                paginatedOptions);

            var count = await _campaignRepository.CountWhereAsync(
                x => x.Categories.Any(p => categories.Contains(p.Id)) && x.Status == CampaignStatus.Live);

            var campaignIds = result.Select(x => x.Id);

            var campaignPartners = await _campaignPartnerRepository.FindWhereAsync(x =>
                campaignIds.Contains(x.Campaign.Id) && x.ContentCreator.Id == user.Person.Id, null);

            var res = result.GroupJoin(campaignPartners, x => x.Id, x => x.Campaign.Id, (campaign, partners) => new { campaign, partners})
                .Select(x =>
                {
                    var mapped = _mapper.Map<CampaignCreatorResult>(x.campaign);
                    if (x.partners.Any())
                    {
                        mapped.Partnership = new CreatorPartnerResult()
                        {
                            Status = x.partners.First().Status,
                            AffiliateCode = x.partners.First().AffiliateCode
                        };
                    };
                    return mapped;
                });

            return new PaginatedResult<CampaignCreatorResult>(paginatedOptions, count, res);
        }
    }
}
