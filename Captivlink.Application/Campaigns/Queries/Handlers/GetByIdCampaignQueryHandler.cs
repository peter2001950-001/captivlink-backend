using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetByIdCampaignQueryHandler : IQueryHandler<GetByIdCampaignQuery, CampaignBusinessResult?>

    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;
        private readonly IPerformanceCacheService _performanceCacheService;

        public GetByIdCampaignQueryHandler(IUserRepository userRepository, IMapper mapper, ICampaignRepository campaignRepository, ICampaignEventRepository campaignEventRepository, IPerformanceCacheService performanceCacheService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _campaignRepository = campaignRepository;
            _performanceCacheService = performanceCacheService;
        }

        public async Task<CampaignBusinessResult?> Handle(GetByIdCampaignQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user?.Company == null)
            {
                return null;
            }

            var result = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Company.Id == user.Company.Id && x.Id == request.CampaignId);
            if (result == null) return null;

            var mappedResult = _mapper.Map<CampaignBusinessResult>(result);
            
            var performance = await _performanceCacheService.GetCampaignPerformanceAsync(result.Id);
            mappedResult.TotalClicks = performance.ClicksCount;
            mappedResult.TotalPurchases = performance.PurchasesCount;
            mappedResult.TotalPurchaseValue = performance.TotalValue;

            return mappedResult;
        }
    }
}
