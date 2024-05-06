using Captivlink.Application.Commons;
using Captivlink.Application.Campaigns.Results;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetAllCampaignQuery : PaginatedQuery<CampaignBusinessResult>
    {
        public Guid UserId { get; set; }
    }
}
