using AutoMapper;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Application.Validators.Services;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using FluentValidation.Results;

namespace Captivlink.Application.Users.Commands.Handlers
{
    public class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public UpdateProfileCommandHandler(IUserRepository userRepository, IMapper mapper, ICategoryRepository categoryRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }
        public async Task<ValueResult<bool>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                return new ValueResult<bool>(new ValidationFailure("userId", "User does not exist"));
            }

            if (request.PersonDetails != null)
            {
                user.Person ??= new PersonDetails();
                _mapper.Map(request.PersonDetails, user.Person);
                user.Person.Categories = await _categoryRepository.GetCategoriesFromListAsync(request.PersonDetails.Categories);
            }
            if (request.CompanyDetails != null)
            {
                user.Company ??= new CompanyDetails();
                _mapper.Map(request.CompanyDetails, user.Company);
            }
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;

            await _userRepository.UpdateAsync(user);
            return new ValueResult<bool>(true);
        }
    }
}
