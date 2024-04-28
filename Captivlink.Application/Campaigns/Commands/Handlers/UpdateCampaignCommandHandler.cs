using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Campaigns.Commands.Handlers
{
    public class UpdateCampaignCommandHandler : ICommandHandler<UpdateCampaignCommand, bool>
    {
        private readonly ICampaignRepository _campaignRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;

        public UpdateCampaignCommandHandler(ICampaignRepository campaignRepository, IWebsiteRepository websiteRepository, IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            _campaignRepository = campaignRepository;
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ValueResult<bool>> Handle(UpdateCampaignCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new(new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }

            var campaign = await _campaignRepository.GetFirstOrDefaultAsync(
                    x => x.Id == request.Id && x.Company.Id == user.Company.Id);
            if (campaign == null)
            {
                return new(new ValidationFailure("id", "Campaign does not exist"));
            }

            if (campaign.Status == CampaignStatus.Live)
            {
                var update = await CommonUpdate(request, campaign, user);
                if (!update.IsValid)
                    return new(update.Errors);

                if (campaign.BudgetPerCreator < request.BudgetPerCreator)
                {
                    campaign.BudgetPerCreator = request.BudgetPerCreator;
                }
            }
            else if(campaign.Status == CampaignStatus.Draft)
            {
                var update = await CommonUpdate(request, campaign, user);
                if (!update.IsValid)
                    return new(update.Errors);

                campaign.BudgetPerCreator = request.BudgetPerCreator;
                campaign.Status = request.Status;
                campaign.EndDateTime = request.EndDateTime;

                UpdateAwards(request, campaign);
            }

            await _campaignRepository.UpdateAsync(campaign);
            return new();
        }

        private async Task<ValueResult> CommonUpdate(UpdateCampaignCommand request, Campaign campaign, ApplicationUser user)
        {

            campaign.InternalName = request.InternalName;
            campaign.ExternalName = request.ExternalName;
            campaign.Description = request.Description;
            campaign.Categories = await _categoryRepository.GetCategoriesFromListAsync(request.Categories);
            campaign.Images = request.Images;
            campaign.Link = request.Link;
            if (request.WebsiteId != campaign.Website.Id)
            {
                var website = await _websiteRepository.GetFirstOrDefaultAsync(x =>
                    x.Id == request.WebsiteId && x.Company.Id == user.Company!.Id);
                if (website == null)
                {
                    return new(new ValidationFailure("websiteId", "Website is not found"));
                }
                campaign.Website = website;
            }

            return new();
        }

        private void UpdateAwards(UpdateCampaignCommand request, Campaign campaign)
        {
            if (campaign.Awards.Count == 0)
            {
                return;
            }

            foreach (var awardEntity in campaign.Awards.ToList())
            {
                var requestEntity = request.Awards.Find(x => x.Id == awardEntity.Id);
                if (requestEntity == null)
                {
                    campaign.Awards.Remove(awardEntity);
                }
                else
                {
                    awardEntity.Amount = requestEntity.Amount;
                    awardEntity.Type  = requestEntity.Type;
                }
            }

            foreach (var awardRequest in request.Awards.Where(x=>x.Id == Guid.Empty))
            {
                campaign.Awards.Add(new Award()
                {
                    Amount = awardRequest.Amount,
                    Type = awardRequest.Type
                });
            }
        }
        
    }
}
