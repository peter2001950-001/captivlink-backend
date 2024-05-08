using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public class RevokeCampaignValidator : AbstractValidator<RevokeCampaignCommand>
    {
        public RevokeCampaignValidator()
        {
            RuleFor(x => x.CampaignId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
