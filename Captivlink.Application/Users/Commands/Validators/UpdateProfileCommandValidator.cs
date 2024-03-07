using Captivlink.Application.Commons;
using FluentValidation;

namespace Captivlink.Application.Users.Commands.Validators
{
    public class UpdateProfileCommandValidator : BaseValidator<UpdateProfileCommand>
    {
        public UpdateProfileCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty();
            RuleFor(x => x.PersonDetails).NotNull().When(x => x.UserRole == "ContentCreator");
            RuleFor(x => x.PersonDetails).Null().When(x => x.UserRole == "Business");
            RuleFor(x => x.CompanyDetails).NotNull().When(x => x.UserRole == "Business");
            RuleFor(x => x.CompanyDetails).Null().When(x => x.UserRole == "ContentCreator");
            RuleFor(x => x.PersonDetails).SetValidator(new PersonModelValidator()!).When(x => x.PersonDetails != null);
            RuleFor(x => x.CompanyDetails).SetValidator(new CompanyModelValidator()!).When(x => x.CompanyDetails != null);
        }
    }
}
