using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<Category> Query  => DbContext.Categories;

        public async Task<List<Category>> GetCategoriesFromListAsync(List<Guid> list)
        {
            return await Query.Where(x => list.Contains(x.Id)).ToListAsync();
        }
    }
}
