using Captivlink.Infrastructure.Domain.Enums;

namespace Captivlink.Api.Models.Requests
{
    public class CampaignPartnershipRequest : PaginationRequest
    {
        public CampaignPartnerStatus? Status { get; set; }
    }
}
