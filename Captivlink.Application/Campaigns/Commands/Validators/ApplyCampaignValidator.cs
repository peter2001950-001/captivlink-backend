using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public class ApplyCampaignValidator : AbstractValidator<ApplyCampaignCommand>
    {
        public ApplyCampaignValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.CampaignId).NotEmpty();

        }
    }
}
