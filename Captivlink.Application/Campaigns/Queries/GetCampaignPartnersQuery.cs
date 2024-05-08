using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Commons;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCampaignPartnersQuery : PaginatedQuery<CampaignPartnerResult>
    {
        public Guid UserId { get; set; }
        public Guid CampaignId { get; set; }
    }
}
