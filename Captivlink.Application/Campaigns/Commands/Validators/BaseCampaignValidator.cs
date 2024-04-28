using Captivlink.Application.Commons;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Captivlink.Application.Campaigns.Commands.Validators
{
    public abstract class BaseCampaignValidator<T> : BaseValidator<T> where T : BaseCampaignCommand<bool>
    {
        protected BaseCampaignValidator(IServiceProvider serviceProvider)
        {
            var scope = serviceProvider.CreateScope();
            var websiteRepository = scope.ServiceProvider.GetService<IWebsiteRepository>();

            RuleFor(x => x.InternalName).NotEmpty().MaximumLength(250);
            RuleFor(x => x.ExternalName).NotEmpty().MaximumLength(250);
            RuleFor(x => x.Description).NotEmpty().MaximumLength(2000);
            RuleFor(x => x.Categories).NotEmpty();;
            RuleFor(x => x.Categories).ForEach(x => x.NotEmpty()).WithMessage("Guid should not be the default");
            RuleFor(x => x.Awards).NotEmpty().Custom((list, context) =>
            {
                if (list.Count(x => x.Type == AwardType.PerClick) > 1)
                {
                    context.AddFailure("Awards", "Only one award with type 'PerClick' is allowed");
                }
                if (list.Count(x => x.Type == AwardType.PerConversion) > 1)
                {
                    context.AddFailure("Awards", "Only one award with type 'PerConversion' is allowed");
                }
                if (list.Count(x => x.Type == AwardType.Percentage) > 1)
                {
                    context.AddFailure("Awards", "Only one award with type 'Percentage' is allowed");
                }
            });
            RuleFor(x => x.BudgetPerCreator).GreaterThan(0.01m);
            RuleFor(x => x).Custom((obj, context) =>
            {
                var sum = obj.Awards.Where(x => x.Type == AwardType.PerClick && x.Type == AwardType.PerConversion)
                    .Sum(x => x.Amount);

                if (obj.BudgetPerCreator < sum * 5)
                {
                    context.AddFailure("BudgetPerCreator", $"Budget per creator should be at least {sum}");
                }
            });

            RuleFor(x => x).CustomAsync(async (entity, context, ctx) =>
            {
                var website = await websiteRepository!.GetFirstOrDefaultAsync(x => x.Id == entity.WebsiteId);
                if (website == null)
                {
                    context.AddFailure("WebsiteId", "Website is not found");
                    return;
                }

                if (!entity.Link.Contains(website.Domain))
                {
                    context.AddFailure("Link", "Link is not valid as it is not from the specified website");
                }
            });

            RuleFor(x => x.Images).NotEmpty();
            RuleFor(x => x.Link).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.WebsiteId).NotEmpty();
        }

    }
}
