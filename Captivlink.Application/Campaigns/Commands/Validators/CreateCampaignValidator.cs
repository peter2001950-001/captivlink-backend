using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public class CreateCampaignValidator: BaseCampaignValidator<CreateCampaignCommand>
    {
        public CreateCampaignValidator(IServiceProvider serviceProvider) : base(serviceProvider)
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
            });

            RuleFor(x => x.EndDateTime).GreaterThan(DateTime.Now.AddDays(10)).WithMessage("End date should be at least 10 days from now");
        }
    }
}
