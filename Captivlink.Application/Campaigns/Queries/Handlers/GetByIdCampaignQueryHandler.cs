using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetByIdCampaignQueryHandler : IQueryHandler<GetByIdCampaignQuery, CampaignResult?>

    {
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IMapper _mapper;

        public GetByIdCampaignQueryHandler(IUserRepository userRepository, IMapper mapper, ICampaignRepository campaignRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _campaignRepository = campaignRepository;
        }

        public async Task<CampaignResult?> Handle(GetByIdCampaignQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user?.Company == null)
            {
                return null;
            }

            var result = await _campaignRepository.GetFirstOrDefaultAsync(x => x.Company.Id == user.Company.Id && x.Id == request.CampaignId);
            
            return result == null ? null : _mapper.Map<CampaignResult>(result);
        }
    }
}
