using System.Text;
using System.Text.RegularExpressions;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Campaigns.Commands.Handlers
{
    public class ApproveOrRejectPartnerCommandHandler : ICommandHandler<ApproveOrRejectPartnerCommand, bool>
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICampaignRepository _campaignRepository;

        public ApproveOrRejectPartnerCommandHandler(ICampaignPartnerRepository campaignPartnerRepository, IUserRepository userRepository, ICampaignRepository campaignRepository)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _userRepository = userRepository;
            _campaignRepository = campaignRepository;
        }

        public async Task<ValueResult<bool>> Handle(ApproveOrRejectPartnerCommand request, CancellationToken cancellationToken)
        {
            var user =  await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new(new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }

            var partner = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x => x.Id == request.CampaignPartnerId && x.Campaign.Company.Id == user.Company.Id);
            if (partner == null)
            {
                return new(new ValidationFailure("campaignPartnerId", "Partnership does not exist"));
            }

            if (request.Outcome == PartnerOutcome.Approve)
            {
                partner.Status = CampaignPartnerStatus.Approved;
                partner.AffiliateCode = await GenerateAffiliateCodeAsync(partner.ContentCreator);
            }
            else
            {
                partner.Status = CampaignPartnerStatus.Rejected;
            }

            await _campaignPartnerRepository.UpdateAsync(partner);
            return new ValueResult<bool>();
        }

        private async Task<string> GenerateAffiliateCodeAsync(PersonDetails contentCreator)
        {
            string result = Regex.Replace(contentCreator.Nickname, "[^a-zA-Z]", "");

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            int length = 3;
            StringBuilder randomString = new StringBuilder(result);
            randomString.Append("_");

            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                randomString.Append(chars[random.Next(chars.Length)]);
            }

            var count = await _campaignPartnerRepository.CountWhereAsync(x => x.AffiliateCode == randomString.ToString());
            if (count > 0)
            {
                return await GenerateAffiliateCodeAsync(contentCreator);
            }
            return randomString.ToString();
        }
    }
}
