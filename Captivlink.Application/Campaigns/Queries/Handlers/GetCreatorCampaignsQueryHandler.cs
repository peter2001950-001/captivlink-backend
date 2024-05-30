using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCreatorCampaignsQueryHandler : IQueryHandler<GetCreatorCampaignsQuery, PaginatedResult<CampaignCreatorResult>?>
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IPerformanceCacheService _performanceCacheService;

        public GetCreatorCampaignsQueryHandler(ICampaignPartnerRepository campaignPartnerRepository, IMapper mapper, IUserRepository userRepository, IPerformanceCacheService performanceCacheService)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _performanceCacheService = performanceCacheService;
        }

        public async Task<PaginatedResult<CampaignCreatorResult>?> Handle(GetCreatorCampaignsQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            if (user == null || user.Person == null)
            {
                return null;
            }

            var paginatedResult =
                await _campaignPartnerRepository.GetPagedCreatorCampaignsAsync(user.Person.Id, paginatedOptions,
                    request.Status);

            var result = paginatedResult.Data.Select(async x =>
            {
                var obj = _mapper.Map<CampaignCreatorResult>(x.Campaign);
                obj.Partnership = new CreatorPartnerResult()
                {
                    AffiliateCode = x.AffiliateCode,
                    Status = x.Status
                };
                obj.AmountEarned = (await _performanceCacheService.GetCreatorCampaignPerformance(x.Id)).TotalEarned;
                return obj;
            }).Select(x => x.Result);

            return new PaginatedResult<CampaignCreatorResult>(paginatedOptions, paginatedResult.TotalCount, result);
        }
    }
}