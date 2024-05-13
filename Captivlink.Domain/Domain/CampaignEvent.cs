using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Infrastructure.Domain
{
    public class CampaignEvent : Entity
    {
        public string? IpAddress { get; set; }
        public string SessionId { get; set; }
        public CampaignPartner CampaignPartner { get; set; }
        public DateTime ProcessedOn { get; set; }
        public CampaignEventType Type { get; set; }
        public double? Amount { get; set; }
        public string? ExternalId  { get; set; }

    }
}
