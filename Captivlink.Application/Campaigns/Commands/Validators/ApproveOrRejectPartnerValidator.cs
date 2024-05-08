using FluentValidation;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public class ApproveOrRejectPartnerValidator : AbstractValidator<ApproveOrRejectPartnerCommand>
    {
        public ApproveOrRejectPartnerValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CampaignPartnerId).NotEmpty();
            RuleFor(x => x.Outcome).IsInEnum();

        }
    }
}
