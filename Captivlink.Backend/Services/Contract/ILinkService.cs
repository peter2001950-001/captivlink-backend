using Captivlink.Infrastructure.Domain;

namespace Captivlink.Backend.Services.Contract
{
    public interface ILinkService
    {
        Task<CampaignPartner?> GetCampaignPartnerAsync(string affCode);
    }
}
