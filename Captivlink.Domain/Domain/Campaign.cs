using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Infrastructure.Domain
{
    public class Campaign : Entity
    {
        public IReadOnlyCollection<string> Images { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public string EventName { get; set; }
        public ICollection<Award> Awards { get; set; }
        public string Link { get; set; }
        public decimal BudgetPerCreator { get; set; }
        public DateTime EndDateTime { get; set; }
        public virtual Website Website { get; set; }
        public CompanyDetails Company { get; set; }
        public CampaignStatus Status { get; set; }
    }
}
