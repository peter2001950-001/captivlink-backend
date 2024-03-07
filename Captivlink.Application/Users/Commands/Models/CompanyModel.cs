using Captivlink.Infrastructure.Domain;

namespace Captivlink.Application.Users.Commands.Models
{
    public class CompanyModel
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
