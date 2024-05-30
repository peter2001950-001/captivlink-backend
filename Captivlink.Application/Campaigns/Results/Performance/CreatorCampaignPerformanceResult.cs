using Captivlink.Application.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Campaigns.Results.Performance
{
    public class CreatorCampaignPerformanceResult
    {
        public MetricsResult MetricsForPeriod { get; set; }
        public MetricsResult MetricsLifetime { get; set; }
        public ChartResult ClickData { get; set; }
        public ChartResult PurchaseData { get; set; }
        public ChartResult PurchaseValueData { get; set; }
    }
}
