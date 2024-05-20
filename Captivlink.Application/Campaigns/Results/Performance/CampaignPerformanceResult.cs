using Captivlink.Application.Commons;

namespace Captivlink.Application.Campaigns.Results.Performance
{
    public class CampaignPerformanceResult
    {
        public MetricsResult MetricsForPeriod { get; set; }
        public MetricsResult MetricsLifetime { get; set; }
        public int TotalPartnerships { get; set; }
        public ChartResult ClickData { get; set; }
        public ChartResult PurchaseData { get; set; }
        public ChartResult PurchaseValueData { get; set; }
      
    }

}
