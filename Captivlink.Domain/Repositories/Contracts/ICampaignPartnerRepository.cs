using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Infrastructure.Repositories.Contracts
{
    public interface ICampaignPartnerRepository : IBaseRepository<CampaignPartner>
    {
        Task<PaginatedResult<CampaignPartner>> GetPagedCreatorCampaignsAsync(Guid creatorId, PaginationOptions paginationOptions, CampaignPartnerStatus? status);
        Task<CampaignPartner?> GetCampaignPartnerByAffCodeAsync(string affiliateCode);
        Task<CampaignPartner?> GetCampaignPartnerByIdAsync(Guid campaignCreatorId);
    }
}
