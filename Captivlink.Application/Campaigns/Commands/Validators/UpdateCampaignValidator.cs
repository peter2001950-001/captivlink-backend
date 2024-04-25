using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public class UpdateCampaignValidator : BaseCampaignValidator<UpdateCampaignCommand>
    {
        public UpdateCampaignValidator(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var campaignRepository = scope.ServiceProvider.GetService<ICampaignRepository>();
            var userRepository = scope.ServiceProvider.GetService<IUserRepository>();

            RuleFor(x => x).CustomAsync(async (entity, context, ctx) =>
            {
                var user = await userRepository!.GetUserById(entity.UserId);
                if (user == null)
                {
                    context.AddFailure("UserId", "User is not found");
                    return;
                }

                if (user.Company == null)
                {
                    context.AddFailure("UserId", "User is not activated");
                    return;
                }

                var campaings = await campaignRepository!.CountWhereAsync(x => x.EventName == entity.EventName && x.Company.Id == user.Company.Id && x.Id != entity.Id);
                if (campaings > 0)
                {
                    context.AddFailure("EventName", "Event name must be unique");
                }
            });
        }
    }
}
