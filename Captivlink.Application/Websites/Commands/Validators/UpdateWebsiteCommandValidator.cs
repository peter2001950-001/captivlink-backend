using FluentValidation;

namespace Captivlink.Application.Websites.Commands.Validators
{
    public class UpdateWebsiteCommandValidator : BaseWebsiteCommandValidator<UpdateWebsiteCommand>
    {
        public UpdateWebsiteCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
}
