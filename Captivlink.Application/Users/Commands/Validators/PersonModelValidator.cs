using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Users.Commands.Models;
using FluentValidation;

namespace Captivlink.Application.Users.Commands.Validators
{
    public class PersonModelValidator: AbstractValidator<PersonModel>
    {
        public PersonModelValidator()
        {
            RuleFor(x => x.Summary).NotEmpty().Length(0, 250);
            RuleFor(x => x.Description).NotEmpty().Length(0, 2500);
            RuleFor(x => x.Nickname).NotEmpty().Length(0, 80);
            RuleFor(x => x.SocialMediaLinks).NotEmpty().Must(x => x.Count < 10).WithMessage("Social links count should be less than 10");
            RuleFor(x => x.Avatar).NotEmpty();
            RuleFor(x => x.Nationality).NotEmpty().Length(0, 3);
            RuleFor(x => x.Categories).NotEmpty();
        }
    }
}
