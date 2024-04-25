using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Application.Campaigns.Results
{
    public class AwardResult
    {
        public Guid Id { get; set; }
        public AwardType Type { get; set; }
        public decimal Amount { get; set; }
    }
}
