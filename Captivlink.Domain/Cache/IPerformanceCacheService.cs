using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Cache.Models;

namespace Captivlink.Infrastructure.Cache
{
    public interface IPerformanceCacheService
    {
        Task CampaignEventAdded(Guid campaignId);
        Task<CampaignPerformance> GetCampaignPerformanceAsync(Guid campaignId);
        Task<CreatorCampaignPerformance> GetCreatorCampaignPerformance(Guid campaignPartnerId);
    }
}
