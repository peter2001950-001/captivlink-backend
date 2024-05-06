using Captivlink.Application.Campaigns.Results;
using Captivlink.Application.Commons;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCampaignFeedQuery : PaginatedQuery<CampaignCreatorResult>
    {
        public Guid UserId { get; set; }
    }
}
