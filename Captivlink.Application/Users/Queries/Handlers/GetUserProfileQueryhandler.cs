using AutoMapper;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Application.Users.Queries.Handlers
{
    public class GetUserProfileQueryHandler : IQueryHandler<GetUserProfileQuery, UserProfileResult?>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public GetUserProfileQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            this._mapper = mapper;
        }

        public async Task<UserProfileResult?> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);

            if (user == null) return null;

            return _mapper.Map<UserProfileResult>(user);
        }
    }
}
