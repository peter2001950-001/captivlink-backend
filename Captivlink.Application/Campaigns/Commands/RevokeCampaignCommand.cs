using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;

namespace Captivlink.Application.Campaigns.Commands
{
    public class RevokeCampaignCommand : ICommand<bool>
    {
        public Guid CampaignId { get; set; }
        public Guid UserId { get; set; }
    }
}
