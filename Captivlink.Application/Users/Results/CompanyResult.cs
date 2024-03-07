using Captivlink.Infrastructure.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Users.Results
{
    public class CompanyResult
    {
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public string IdentificationNumber { get; set; }
        public string CountryOfRegistration { get; set; }
        public string Address { get; set; }
        public string BeneficialOwner { get; set; }
        public string Website { get; set; }
        public List<SocialLink> SocialMediaLinks { get; set; }
    }
}
