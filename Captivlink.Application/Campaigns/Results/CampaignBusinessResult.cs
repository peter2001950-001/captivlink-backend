using Captivlink.Application.Campaigns.Commands.Models;
using Captivlink.Infrastructure.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Websites.Results;
using Captivlink.Application.Commons;

namespace Captivlink.Application.Campaigns.Results
{
    public class CampaignBusinessResult
    {
        public Guid Id { get; set; }
        public List<string> Images { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Description { get; set; }
        public List<CategoryResult> Categories { get; set; }
        public string EventName { get; set; }
        public List<AwardResult> Awards { get; set; }
        public string Link { get; set; }
        public decimal BudgetPerCreator { get; set; }
        public DateTime EndDateTime { get; set; }
        public WebsiteResult Website { get; set; }
        public CampaignStatus Status { get; set; }
        public int TotalClicks { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalPurchaseValue { get; set; }
    }
}
