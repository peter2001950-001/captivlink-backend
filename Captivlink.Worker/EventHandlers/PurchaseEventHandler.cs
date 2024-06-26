using Captivlink.Infrastructure.Cache;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Events;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Worker.Interfaces;
using Newtonsoft.Json;

namespace Captivlink.Worker.EventHandlers
{
    public class PurchaseEventHandler : IEventHandler
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;
        private readonly ICampaignEventRepository _campaignEventRepository;
        private readonly IPerformanceCacheService _performanceCacheService;
        private static readonly LockProvider<string> LockProvider = new LockProvider<string>();

        public PurchaseEventHandler(ICampaignPartnerRepository campaignPartnerRepository, ICampaignEventRepository campaignEventRepository, IPerformanceCacheService performanceCacheService)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
            _campaignEventRepository = campaignEventRepository;
            _performanceCacheService = performanceCacheService;
        }

        public string EventType => "PurchaseEvent";
        public async Task HandleAsync(string value)
        {
            var obj = JsonConvert.DeserializeObject<PurchaseEvent>(value, new JsonSerializerSettings() { DateTimeZoneHandling = DateTimeZoneHandling.Utc });
            if (obj == null)
                return;

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

                if (await _campaignEventRepository.GetFirstOrDefaultAsync(x =>
                        x.CampaignPartner.Id == campaignPartner.Id && x.Type == CampaignEventType.Purchase &&
                        (x.ExternalId == obj.Identifier || x.SessionId == trackingIdentifier.SessionId)) != null)
                    return;

                var purchaseEvent = new CampaignEvent()
                {
                    Amount = (double)(obj.Amount ?? 0),
                    CampaignPartner = campaignPartner,
                    CreatedOn = obj.CreatedOn,
                    ProcessedOn = DateTime.UtcNow,
                    ExternalId = obj.Identifier,
                    Id = Guid.NewGuid(),
                    IpAddress = obj.IpAddress,
                    SessionId = trackingIdentifier.SessionId,
                    Type = CampaignEventType.Purchase
                };

                await _campaignEventRepository.AddAsync(purchaseEvent);
                await _performanceCacheService.CampaignEventAdded(purchaseEvent.CampaignPartner.Campaign.Id);
            }
            finally
            {
                LockProvider.Release(obj.AffCode);
            }
        }
    }
}
