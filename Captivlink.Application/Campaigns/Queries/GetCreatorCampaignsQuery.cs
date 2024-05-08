using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Commons;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCreatorCampaignsQuery : PaginatedQuery<CampaignCreatorResult>
    {
        public CampaignPartnerStatus? Status { get; set; }
        public Guid UserId { get; set; }
    }
}
