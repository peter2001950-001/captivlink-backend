using Captivlink.Application.Interfaces.ValidatorPipelines.Commands;

namespace Captivlink.Application.Campaigns.Commands
{
    public class ApproveOrRejectPartnerCommand : ICommand<bool>
    {
        public Guid CampaignPartnerId { get; set; }
        public Guid UserId { get; set; }
        public PartnerOutcome Outcome { get; set; }
    }

    public enum PartnerOutcome
    {
        Approve,
        Reject
    }
}
