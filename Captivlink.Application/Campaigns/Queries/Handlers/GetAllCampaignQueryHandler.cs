using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Websites.Queries;
using Captivlink.Application.Websites.Results;
using Captivlink.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Campaigns.Queries.Handlers
{
    public class GetAllCampaignQueryHandler : IQueryHandler<GetAllCampaignQuery, PaginatedResult<CampaignResult>?>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetAllCampaignQueryHandler(ICampaignRepository campaignRepository, IUserRepository userRepository, IMapper mapper)
        {
            this._campaignRepository = campaignRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<CampaignResult>?> Handle(GetAllCampaignQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var result = await _campaignRepository.FindWhereAsync(x => x.Company.Id == user.Company.Id, paginatedOptions);
            var count = await _campaignRepository.CountWhereAsync(x => x.Company.Id == user.Company.Id);

            return new PaginatedResult<CampaignResult>(paginatedOptions, count,
                _mapper.Map<IEnumerable<CampaignResult>>(result));
        }
    }
}
