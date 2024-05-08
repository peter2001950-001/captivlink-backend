using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCampaignPartnersQueryHandler : IQueryHandler<GetCampaignPartnersQuery, PaginatedResult<CampaignPartnerResult>?>
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;

        public GetCampaignPartnersQueryHandler(ICampaignPartnerRepository campaignPartnerRepository, IUserRepository userRepository, ICampaignRepository campaignRepository, IMapper mapper)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
            _mapper = mapper;
        }

        public  async Task<PaginatedResult<CampaignPartnerResult>?> Handle(GetCampaignPartnersQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x =>
                    x.Id == request.CampaignId && x.Company.Id == user.Company.Id);
            if (campaign == null)
            {
                return null;
            }

            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            var result = await _campaignPartnerRepository.FindWhereAsync(x => x.Campaign.Id == request.CampaignId, paginatedOptions);
            var count = await _campaignPartnerRepository.CountWhereAsync(x => x.Campaign.Id == request.CampaignId);

            return new PaginatedResult<CampaignPartnerResult>(paginatedOptions, count,
                _mapper.Map<IEnumerable<CampaignPartnerResult>>(result));

        }
    }
}
