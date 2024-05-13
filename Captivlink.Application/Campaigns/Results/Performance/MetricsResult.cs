using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captivlink.Application.Campaigns.Results.Performance
{
    public class MetricsResult
    {
        public int TotalClick { get; set; }
        public int TotalPurchases { get; set; }
        public decimal TotalValue { get; set; }
        public decimal TotalAmountSpent { get; set; }
    }
}
