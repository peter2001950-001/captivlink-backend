using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Utility;

namespace Captivlink.Infrastructure.Repositories.Contracts
{
    public interface IBaseRepository<TEntity> where TEntity : Entity
    {
        Task<IEnumerable<TEntity>> FindAllAsync(PaginationOptions? request);

        Task<IEnumerable<TEntity>> FindWhereAsync(Expression<Func<TEntity, bool>> whereExpression,
            PaginationOptions? request);

        Task<PaginatedResult<TEntity>> GetPagedAsync(PaginationOptions request,
            Expression<Func<TEntity, bool>>[] whereExpressions);

        Task<PaginatedResult<TEntity>> GetPagedAsync(PaginationOptions request,
            Expression<Func<TEntity, bool>> whereExpressions);

        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression);
        Task<int> CountAllAsync();
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity?> UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
        Task<int> CountWhereAsync(Expression<Func<TEntity, bool>> whereExpression);
    }
}
