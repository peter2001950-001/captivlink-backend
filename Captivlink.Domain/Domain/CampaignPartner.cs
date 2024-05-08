using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Infrastructure.Domain
{
    public class CampaignPartner : Entity
    {
        public Campaign Campaign { get; set; }
        public PersonDetails ContentCreator { get; set; }
        public CampaignPartnerStatus Status { get; set; }
        public string? AffiliateCode { get; set; }
    }
}
