using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCreatorCampaignByIdQuery : IQuery<CampaignCreatorResult>
    {
        public Guid CampaignId { get; set; }
        public Guid UserId { get; set; }
    }
}
