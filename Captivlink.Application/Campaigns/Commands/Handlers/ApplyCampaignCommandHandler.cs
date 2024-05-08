using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Campaigns.Commands.Handlers
{
    public class ApplyCampaignCommandHandler : ICommandHandler<ApplyCampaignCommand, bool>
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly ICampaignRepository _campaignRepository;
        private readonly IUserRepository _userRepository;

        public ApplyCampaignCommandHandler(ICampaignPartnerRepository campaignPartnerRepository, ICampaignRepository campaignRepository, IUserRepository userRepository)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignRepository = campaignRepository;
            _userRepository = userRepository;
        }

        public async Task<ValueResult<bool>> Handle(ApplyCampaignCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Person == null)
            {
                return new ValueResult<bool>(new ValidationFailure("userId", "User is not found or not activated"));
            }

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(x =>
                x.Id == request.CampaignId && x.Status == CampaignStatus.Live);
            if (campaign == null)
            {
                return new ValueResult<bool>(new ValidationFailure("campaignId", "Active campaign is not found"));
            }

            var campaignPartner = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x =>
                x.Campaign.Id == request.CampaignId && x.ContentCreator.Id == user.Person.Id);
            
            if (campaignPartner != null)
            {
                return new ValueResult<bool>(new ValidationFailure("campaignId", "Already applied for this campaign"));
            }

            var entity = new CampaignPartner()
            {
                Campaign = campaign,
                ContentCreator = user.Person,
                Status = CampaignPartnerStatus.AwaitingApproval
            };

           await _campaignPartnerRepository.AddAsync(entity);
           return new ValueResult<bool>(true);
        }

       
    }

}
