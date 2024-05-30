namespace Captivlink.Infrastructure.Cache.Models
{
    public class CampaignPerformance
    {
        public int ClicksCount { get; set; }
        public int PurchasesCount { get; set; }
        public decimal TotalValue { get; set; }
        public Dictionary<Guid, decimal> CreatorAmountEarned { get; set; } = new();
    }
}
