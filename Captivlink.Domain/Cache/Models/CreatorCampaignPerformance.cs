using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Infrastructure.Cache.Models
{
    public class CreatorCampaignPerformance
    {
        public int ClicksCount { get; set; }
        public int PurchasesCount { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalEarned { get; set; }
    }
}
