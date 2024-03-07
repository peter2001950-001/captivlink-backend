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
        Task<IEnumerable<TEntity>> FindAllAsync(PaginationOptions request);

        Task<IEnumerable<TEntity>> FindWhereAsync(Expression<Func<TEntity, bool>> whereExpression,
            PaginationOptions? request);

        Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression);
    }
}
