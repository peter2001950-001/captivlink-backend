using Captivlink.Infrastructure.Domain;

namespace Captivlink.PublicApi.Services.Contract
{
    public interface ILinkService
    {
        Task<CampaignPartner?> GetCampaignPartnerAsync(string affCode);
    }
}
