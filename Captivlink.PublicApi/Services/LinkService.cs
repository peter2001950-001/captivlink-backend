using Captivlink.PublicApi.Services.Contract;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;

namespace Captivlink.PublicApi.Services
{
    public class LinkService : ILinkService
    {
        private readonly ICampaignPartnerRepository _campaignPartnerRepository;

        public LinkService(ICampaignPartnerRepository campaignPartnerRepository)
        {
            _campaignPartnerRepository = campaignPartnerRepository;
        }

        public async Task<CampaignPartner?> GetCampaignPartnerAsync(string affCode)
        {
            var campaignPartner = await _campaignPartnerRepository.GetCampaignPartnerByAffCodeAsync(affCode);
            return campaignPartner;
        }
    }
}
