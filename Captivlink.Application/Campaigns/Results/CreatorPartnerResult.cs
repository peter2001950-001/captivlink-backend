using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Application.Campaigns.Results
{
    public class CreatorPartnerResult
    {
        public CampaignPartnerStatus Status { get; set; }
        public string? AffiliateCode { get; set; }
    }
}
