using AutoMapper;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Application.Websites.Results;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Websites.Commands.Handlers
{
    public class CreateWebsiteCommandHandler : ICommandHandler<CreateWebsiteCommand, WebsiteResult>
    {
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public CreateWebsiteCommandHandler(IWebsiteRepository websiteRepository, IUserRepository userRepository, IMapper mapper)
        {
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ValueResult<WebsiteResult>> Handle(CreateWebsiteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new (new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }
            

            var website = _mapper.Map<Website>(request);
            website.Company = user.Company;

            if (await _websiteRepository.IsWebsiteExisting(website))
            {
                return new(new ValidationFailure("website", "Website is already existing"));
            }
            await _websiteRepository.AddAsync(website);

            return new(_mapper.Map<WebsiteResult>(website));
        }
    }
}
