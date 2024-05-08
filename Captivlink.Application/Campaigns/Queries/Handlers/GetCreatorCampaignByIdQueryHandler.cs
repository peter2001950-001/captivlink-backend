using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetCreatorCampaignByIdQueryHandler: IQueryHandler<GetCreatorCampaignByIdQuery, CampaignCreatorResult?>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetCreatorCampaignByIdQueryHandler(ICampaignRepository campaignRepository, ICampaignPartnerRepository campaignPartnerRepository, IUserRepository userRepository, IMapper mapper)
        {
            _campaignRepository = campaignRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<CampaignCreatorResult?> Handle(GetCreatorCampaignByIdQuery request, CancellationToken cancellationToken)
        {
            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x =>
                    x.Id == request.CampaignId && x.Status != CampaignStatus.Draft);

            var response = _mapper.Map<CampaignCreatorResult>(campaign);

            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Person == null)
            {
                return null;
            }

            var partnership = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x =>
                x.Campaign.Id == request.CampaignId && x.ContentCreator.Id == user.Person.Id);

            if (partnership != null)
            {
                response.Partnership = new CreatorPartnerResult()
                {
                    AffiliateCode = partnership.AffiliateCode,
                    Status = partnership.Status
                };
            }

            return response;
        }
    }
}
