using Captivlink.Application.Commons;
using FluentValidation;

namespace Captivlink.Application.Websites.Commands.Validators
{
    public class DeleteWebsiteCommandValidator : BaseValidator<DeleteWebsiteCommand>
    {
        public DeleteWebsiteCommandValidator()
        {
            RuleFor(x => x.WebsiteId).NotEmpty();
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
