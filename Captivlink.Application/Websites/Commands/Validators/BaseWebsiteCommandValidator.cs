using System.Text.RegularExpressions;
using Captivlink.Application.Commons;
using Captivlink.Application.Websites.Results;
using FluentValidation;

namespace Captivlink.Application.Websites.Commands.Validators
{
    public class BaseWebsiteCommandValidator<T> : BaseValidator<T> where T : BaseWebsiteCommand<WebsiteResult>
    {

        public BaseWebsiteCommandValidator()
        {
            Regex rg = new Regex("(?:[a-z0-9](?:[a-z0-9-]{0,61}[a-z0-9])?\\.)+[a-z0-9][a-z0-9-]{0,61}[a-z0-9]");

            RuleFor(x => x.Name).NotEmpty();
            RuleFor(x => x.Name).MaximumLength(250);
            RuleFor(x => x.Domain).Must(x => rg.IsMatch(x)).WithMessage("Not valid domain name");
            RuleFor(x => x.UserId).NotEmpty();
        }
    }
}
