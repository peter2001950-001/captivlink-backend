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

        public GetCampaignFeedQueryHandler(ICampaignRepository campaignRepository, IUserRepository userRepository, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _mapper = mapper;
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

            return new PaginatedResult<CampaignCreatorResult>(paginatedOptions, count,
                _mapper.Map<IEnumerable<CampaignCreatorResult>>(result));
        }
    }
}
