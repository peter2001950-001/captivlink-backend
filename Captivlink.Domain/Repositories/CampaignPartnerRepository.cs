using System.Linq.Dynamic.Core;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Domain.Enums;
using Captivlink.Infrastructure.Repositories.Contracts;
using Captivlink.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CampaignPartnerRepository : BaseRepository<CampaignPartner>, ICampaignPartnerRepository
    {
        public CampaignPartnerRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<CampaignPartner> Query => DbContext.CampaignPartners.Include(x => x.Campaign).Include(x => x.ContentCreator).AsQueryable();

        public async Task<PaginatedResult<CampaignPartner>> GetPagedCreatorCampaignsAsync(Guid creatorId, PaginationOptions paginationOptions, CampaignPartnerStatus? status)
        {
            IQueryable<CampaignPartner> query = Query.Where(x=>x.ContentCreator.Id == creatorId && !x.IsDeleted).Include(x => x.Campaign).ThenInclude(x => x.Company).Include(x => x.Campaign)
                .ThenInclude(x => x.Awards).Include(x => x.Campaign).ThenInclude(x => x.Categories);

            if (status.HasValue)
            {
                query = query.Where(x=>x.Status == status.Value).AsQueryable();
            }

            var items = await query.Sorted(paginationOptions).Paged(paginationOptions).ToListAsync();
            var totalCount = await query.CountAsync();

            return new PaginatedResult<CampaignPartner>(paginationOptions, totalCount, items);

        }
    }
}
