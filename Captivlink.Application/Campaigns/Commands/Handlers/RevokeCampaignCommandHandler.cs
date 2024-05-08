using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Campaigns.Commands.Handlers
{
    public class RevokeCampaignCommandHandler : ICommandHandler<RevokeCampaignCommand, bool>
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IUserRepository _userRepository;
        public RevokeCampaignCommandHandler(ICampaignPartnerRepository campaignPartnerRepository, IUserRepository userRepository)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _userRepository = userRepository;
        }

        public async Task<ValueResult<bool>> Handle(RevokeCampaignCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Person == null)
            {
                return new ValueResult<bool>(new ValidationFailure("userId", "User is not found or not activated"));
            }

            var campaignPartner = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x =>
                x.Campaign.Id == request.CampaignId && x.ContentCreator.Id == user.Person.Id);

            if (campaignPartner == null)
            {
                return new ValueResult<bool>(new ValidationFailure("userId", "Not applied for this campaign yet"));
            }

            if (campaignPartner.Status is CampaignPartnerStatus.Approved or CampaignPartnerStatus.Rejected)
            {
                return new ValueResult<bool>(new ValidationFailure("userId", "The partnership is already approved or rejected and cannot be revoked"));
            }

            await _campaignPartnerRepository.DeleteAsync(campaignPartner.Id);
            return new ValueResult<bool>();
        }
    }
}
