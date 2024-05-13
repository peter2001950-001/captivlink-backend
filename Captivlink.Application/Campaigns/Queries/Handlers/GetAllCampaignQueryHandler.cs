using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Utility;
using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetAllCampaignQueryHandler : IQueryHandler<GetAllCampaignQuery, PaginatedResult<CampaignBusinessResult>?>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICampaignEventRepository _campaignEventRepository;

        public GetAllCampaignQueryHandler(ICampaignRepository campaignRepository, IUserRepository userRepository, IMapper mapper, ICampaignEventRepository campaignEventRepository)
        {
            this._campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _mapper = mapper;
            _campaignEventRepository = campaignEventRepository;
        }

        public async Task<PaginatedResult<CampaignBusinessResult>?> Handle(GetAllCampaignQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var result = await _campaignRepository.FindWhereAsync(x => x.Company.Id == user.Company.Id, paginatedOptions);
            var count = await _campaignRepository.CountWhereAsync(x => x.Company.Id == user.Company.Id);

            var mappedResult = _mapper.Map<List<CampaignBusinessResult>>(result);
            foreach (var campaignBusinessResult in mappedResult)
            {
                var performance = await _campaignEventRepository.GetCampaignPerformanceAsync(campaignBusinessResult.Id);
                campaignBusinessResult.TotalClicks = performance.ClicksCount;
                campaignBusinessResult.TotalPurchases = performance.PurchasesCount;
                campaignBusinessResult.TotalPurchaseValue = performance.TotalValue;
            }

            return new PaginatedResult<CampaignBusinessResult>(paginatedOptions, count, mappedResult);
        }
    }
}
