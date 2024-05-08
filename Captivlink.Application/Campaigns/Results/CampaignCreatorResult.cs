using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Application.Commons;

namespace Captivlink.Application.Campaigns.Results
{
    public class CampaignCreatorResult
    {
        public Guid Id { get; set; }
        public List<string> Images { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Description { get; set; }
        public List<CategoryResult> Categories { get; set; }
        public List<AwardResult> Awards { get; set; }
        public string Link { get; set; }
        public decimal BudgetPerCreator { get; set; }
        public DateTime EndDateTime { get; set; }
        public CampaignStatus Status { get; set; }
        public CompanyResult Company { get; set; }
        public CreatorPartnerResult? Partnership { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
