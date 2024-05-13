using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Users.Results;

namespace Captivlink.Application.Campaigns.Results.Performance
{
    public class CampaignCreatorPerformanceResult
    {
        public PersonResult ContentCreator { get; set; }
        public MetricsResult MetricsForPeriod { get; set; }
        public MetricsResult MetricsLifetime { get; set; }
    }
}
