using AutoMapper;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;
using Captivlink.Application.Websites.Results;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Websites.Queries.Handlers
{
    public class GetAllWebsiteQueryHandler : IQueryHandler<GetAllWebsiteQuery, PaginatedResult<WebsiteResult>?>
    {
        private readonly IWebsiteRepository _websiteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public GetAllWebsiteQueryHandler(IWebsiteRepository websiteRepository, IUserRepository userRepository, IMapper mapper)
        {
            _websiteRepository = websiteRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<PaginatedResult<WebsiteResult>?> Handle(GetAllWebsiteQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            var paginatedOptions = _mapper.Map<PaginationOptions>(request);
            if (user == null || user.Company == null)
            {
                return null;
            }

            var result = await _websiteRepository.FindWhereAsync(x => x.Company.Id == user.Company.Id, paginatedOptions);
            var count = await _websiteRepository.CountWhereAsync(x => x.Company.Id == user.Company.Id);

            return new PaginatedResult<WebsiteResult>(paginatedOptions, count,
                _mapper.Map<IEnumerable<WebsiteResult>>(result));
        }
    }
}
