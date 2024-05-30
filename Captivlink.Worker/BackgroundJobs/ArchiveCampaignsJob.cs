using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.Worker.BackgroundJobs
{
    public class ArchiveCampaignsJob : IScheduledJob
    {
        private readonly ICampaignRepository _campaignRepository;

        public ArchiveCampaignsJob(IServiceProvider serviceProvider)
        {
            this._campaignRepository = serviceProvider.CreateScope().ServiceProvider.GetService<ICampaignRepository>()!;
        }

        public async Task Execute()
        {
            var expiredCampaigns = await _campaignRepository.FindWhereAsync(x =>
                x.EndDateTime < DateTime.UtcNow && x.Status == CampaignStatus.Live, null);

            foreach (var campaign in expiredCampaigns.ToList())
            {
                campaign.Status = CampaignStatus.Completed;
                await _campaignRepository.UpdateAsync(campaign);
            }

            Console.WriteLine("Executed");
        }

        public string CronExpression => "*/10 * * * * *";
    }
}
