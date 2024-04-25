using Captivlink.Application.Campaigns.Commands.Models;
using Captivlink.Infrastructure.Domain.Enums;
using System.Collections.Generic;
using System;

namespace Captivlink.Api.Models.Requests
{
    public class CampaignRequest
    {
        public List<string> Images { get; set; }
        public string InternalName { get; set; }
        public string ExternalName { get; set; }
        public string Description { get; set; }
        public List<Guid> Categories { get; set; }
        public string EventName { get; set; }
        public List<AwardModel> Awards { get; set; }
        public string Link { get; set; }
        public decimal BudgetPerCreator { get; set; }
        public DateTime EndDateTime { get; set; }
        public Guid WebsiteId { get; set; }
        public CampaignStatus Status { get; set; }
    }
}
