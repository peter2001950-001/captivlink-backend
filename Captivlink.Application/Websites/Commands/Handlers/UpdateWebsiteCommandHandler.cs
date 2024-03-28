using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Websites.Results;
using AutoMapper;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Websites.Commands.Handlers
{
    public class UpdateWebsiteCommandHandler: ICommandHandler<UpdateWebsiteCommand, WebsiteResult>
    {
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UpdateWebsiteCommandHandler(IWebsiteRepository websiteRepository, IUserRepository userRepository, IMapper mapper)
        {
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<ValueResult<WebsiteResult>> Handle(UpdateWebsiteCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null || user.Company == null)
            {
                return new(new ValidationFailure("userId", "User does not exist or has not been activated yet"));
            }

            var website = await _websiteRepository.GetFirstOrDefaultAsync(x => x.Id == request.Id);
            if (website == null)
            {
                return new(new ValidationFailure("websiteId", "Website does not exist"));
            }

            _mapper.Map(request, website);
            
            if (await _websiteRepository.IsWebsiteExisting(website))
            {
                return new(new ValidationFailure("", "Website is already existing"));
            }
            await _websiteRepository.UpdateAsync(website);

            return new(_mapper.Map<WebsiteResult>(website));
        }
    }
}
