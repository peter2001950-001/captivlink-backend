using Captivlink.Application.Commons;
using Captivlink.Application.Campaigns.Results;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetAllCampaignQuery : PaginatedQuery<CampaignResult>
    {
        public Guid UserId { get; set; }
    }
}
