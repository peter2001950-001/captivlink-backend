using Captivlink.Infrastructure.Domain;

namespace Captivlink.Infrastructure.Repositories.Contracts
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<Category>> GetCategoriesFromListAsync(List<Guid> list);
    }
}
