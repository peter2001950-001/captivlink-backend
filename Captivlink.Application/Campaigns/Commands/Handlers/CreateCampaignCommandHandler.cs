using AutoMapper;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Campaigns.Commands.Handlers
{
    public class CreateCampaignCommandHandler : ICommandHandler<CreateCampaignCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IWebsiteRepository _websiteRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICampaignRepository _campaignRepository;

        public CreateCampaignCommandHandler(IMapper mapper, IUserRepository userRepository, IWebsiteRepository websiteRepository, ICategoryRepository categoryRepository, ICampaignRepository campaignRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
            _websiteRepository = websiteRepository;
            _categoryRepository = categoryRepository;
            _campaignRepository = campaignRepository;
        }
        public async Task<ValueResult<bool>> Handle(CreateCampaignCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new(new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }

            var entity = _mapper.Map<Campaign>(request);
            var website = await _websiteRepository.GetFirstOrDefaultAsync(x => x.Id == request.WebsiteId && x.Company.Id == user.Company.Id);
            if (website == null)
            {
                return new(new ValidationFailure("websiteId", "Website is not found"));
            }

            entity.Website = website;
            entity.Categories = await _categoryRepository.GetCategoriesFromListAsync(request.Categories);
            entity.Company = user.Company;

            await _campaignRepository.AddAsync(entity);
            return new();
        }
    }
}
