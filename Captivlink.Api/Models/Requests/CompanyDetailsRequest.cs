using Captivlink.Application.Users.Commands.Models;
using Captivlink.Infrastructure.Domain;
using System.Collections.Generic;

namespace Captivlink.Api.Models.Requests
{
    public class CompanyDetailsRequest
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IdentificationNumber { get; set; }
        public string CountryOfRegistration { get; set; }
        public string Address { get; set; }
        public string BeneficialOwner { get; set; }
        public string Website { get; set; }
        public List<SocialLinkModel> SocialMediaLinks { get; set; }
    }
}
