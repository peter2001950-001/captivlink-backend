using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Commons;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCampaignPartnersPerformanceQuery : PaginatedQuery<CampaignPartnerPerformanceResult>
    {
        public Guid CampaignId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
