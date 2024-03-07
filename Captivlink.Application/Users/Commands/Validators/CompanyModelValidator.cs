using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Users.Commands.Models;
using FluentValidation;

namespace Captivlink.Application.Users.Commands.Validators
{
    public class CompanyModelValidator : AbstractValidator<CompanyModel>
    {
        public CompanyModelValidator()
        {
            RuleFor(x => x.Summary).NotEmpty().Length(0, 250);
            RuleFor(x => x.Description).NotEmpty().Length(0, 2500);
            RuleFor(x => x.Address).NotEmpty().Length(0, 250);
            RuleFor(x => x.BeneficialOwner).NotEmpty().Length(0, 250);
            RuleFor(x => x.CountryOfRegistration).NotEmpty().Length(0, 3);
            RuleFor(x => x.IdentificationNumber).NotEmpty().Length(0, 20);
            RuleFor(x => x.Name).NotEmpty().Length(0, 250);
            RuleFor(x => x.Website).NotEmpty().Length(0, 250);
            RuleFor(x => x.SocialMediaLinks).NotEmpty().Must(x => x.Count < 10).WithMessage("Social links count should be less than 10");

        }
    }
}
