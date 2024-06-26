using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Events;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Worker.Interfaces;
using Newtonsoft.Json;

namespace Captivlink.Worker.EventHandlers
{
    public class ClickEventHandler : IEventHandler
    {
        public string EventType => "ClickEvent";
        private readonly ICampaignEventRepository _campaignEventRepository;
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly IPerformanceCacheService _performanceCacheService;
        private static readonly LockProvider<string> LockProvider = new LockProvider<string>();

        public ClickEventHandler(ICampaignEventRepository campaignEventRepository, ICampaignPartnerRepository campaignPartnerRepository, IPerformanceCacheService performanceCacheService)
        {
            _campaignEventRepository = campaignEventRepository;
            _campaignPartnerRepository = campaignPartnerRepository;
            _performanceCacheService = performanceCacheService;
        }

        public async Task HandleAsync(string value)
        {
            var obj = JsonConvert.DeserializeObject<ClickEvent>(value, new JsonSerializerSettings(){DateTimeZoneHandling = DateTimeZoneHandling.Utc});
            if (obj == null)
                return;

            Console.WriteLine("Click handle: " + obj.AffCode);
            await LockProvider.WaitAsync(obj.AffCode);

            try
            {
                var campaignPartner = await _campaignPartnerRepository.GetFirstOrDefaultAsync(x => x.AffiliateCode == obj.AffCode);
                if (campaignPartner == null) return;

                if (campaignPartner.Campaign.EndDateTime < obj.CreatedOn)
                    return;

                var trackingIdentifier = TrackingIdentifier.Parse(obj.SessionId);
                if (trackingIdentifier == null) return;

                if (trackingIdentifier.CampaignCreatorId != campaignPartner.Id) return;

                if (await _campaignEventRepository.GetFirstOrDefaultAsync(x => x.SessionId == trackingIdentifier.SessionId) != null)
                    return;

                if (await _campaignEventRepository.GetFirstOrDefaultAsync(x =>
                        x.CampaignPartner.Id == campaignPartner.Id && x.IpAddress == obj.IpAddress &&
                        Math.Abs((obj.CreatedOn - x.CreatedOn).TotalMinutes) < 1) != null)
                    return;

                var campaignEvent = new CampaignEvent()
                {
                    CreatedOn = obj.CreatedOn,
                    Type = CampaignEventType.Click,
                    CampaignPartner = campaignPartner,
                    SessionId = trackingIdentifier.SessionId,
                    IpAddress = obj.IpAddress,
                    ProcessedOn = DateTime.UtcNow
                };

                await _campaignEventRepository.AddAsync(campaignEvent);
                await _performanceCacheService.CampaignEventAdded(campaignPartner.Campaign.Id);
            }
            finally
            {
                LockProvider.Release(obj.AffCode);
            }
        }
    }
}
