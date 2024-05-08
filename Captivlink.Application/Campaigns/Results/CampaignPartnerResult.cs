using Captivlink.Application.Users.Results;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Application.Campaigns.Results
{
    public class CampaignPartnerResult
    {
        public Guid Id { get; set; }
        public CampaignPartnerStatus Status { get; set; }
        public PersonResult ContentCreator { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
