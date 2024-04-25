namespace Captivlink.Application.Campaigns.Commands
{
    public record UpdateCampaignCommand : BaseCampaignCommand<bool>
    {
        public Guid Id { get; set; }
    }
}
