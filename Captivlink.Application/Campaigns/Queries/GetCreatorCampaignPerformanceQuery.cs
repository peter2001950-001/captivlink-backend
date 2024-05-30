using Captivlink.Application.Campaigns.Results.Performance;
using Captivlink.Application.Interfaces.ValidatorPipelines.Queries;

namespace Captivlink.Application.Campaigns.Queries
{
    public class GetCreatorCampaignPerformanceQuery : IQuery<CreatorCampaignPerformanceResult>
    {
        public Guid CampaignId { get; set; }
        public Guid UserId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
