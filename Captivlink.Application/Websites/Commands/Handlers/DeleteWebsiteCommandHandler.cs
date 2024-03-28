using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Websites.Commands.Handlers
{
    public class DeleteWebsiteCommandHandler : ICommandHandler<DeleteWebsiteCommand, bool>
    {
        private readonly IWebsiteRepository _websiteService;
        private readonly IUserRepository _userRepository;

        public DeleteWebsiteCommandHandler(IWebsiteRepository websiteService, IUserRepository userRepository)
        {
            _websiteService = websiteService;
            _userRepository = userRepository;
        }

        public async Task<ValueResult<bool>> Handle(DeleteWebsiteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new(new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }

            var website = await _websiteService.GetFirstOrDefaultAsync(x => x.Id == request.WebsiteId && x.Company.Id == user.Company.Id);
            if (website == null)
            {
                return new(new ValidationFailure("websiteId", "Website does not exist"));
            }

            await _websiteService.DeleteAsync(website.Id);
            return new ValueResult<bool>(true);
        }
    }
}
