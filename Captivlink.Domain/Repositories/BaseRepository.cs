using System.Linq.Expressions;
using Captivlink.Infrastructure.Data;
using Captivlink.Infrastructure.Domain;
using Captivlink.Infrastructure.Utility;
using Microsoft.EntityFrameworkCore;

namespace Captivlink.Infrastructure.Repositories
{
    public abstract class BaseRepository<TEntity> where TEntity : Entity
    {
        protected readonly ApplicationDbContext DbContext;

        protected BaseRepository(ApplicationDbContext dbContext)
        {
            this.DbContext = dbContext;
        }

        protected abstract IQueryable<TEntity> Query { get; }

        public async Task<IEnumerable<TEntity>> FindAllAsync(PaginationOptions request)
        {
            return await Query.Sorted(request).Paged(request).ToListAsync();
        }

        public async Task<IEnumerable<TEntity>> FindWhereAsync(Expression<Func<TEntity, bool>> whereExpression, PaginationOptions? request)
        {
            return await Query.Where(whereExpression).Sorted(request).Paged(request).ToListAsync();
        }

        public async Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            return await Query.Where(whereExpression).FirstOrDefaultAsync();
        }

        public async Task<int> CountAllAsync()
        {
            return await Query.CountAsync();
        }

        public virtual async Task<TEntity?> GetByIdAsync(Guid id, bool hasTracking = true)
        {
            return await Query.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<TEntity> AddAsync(TEntity entity)
        {
            if (await CanAddAsync(entity))
            {
                entity = await BeforeAddAsync(entity);

                await DbContext.AddAsync(entity);
                await DbContext.SaveChangesAsync();
                await AfterAddAsync(entity);
            }

            return entity;
        }

        public virtual async Task<TEntity?> UpdateAsync(Guid id, TEntity entity)
        {
            entity.Id = id;
            if (await CanUpdateAsync(entity))
            {
                var localEntity = await Query.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);
                if (localEntity == null)
                    return null;

                entity = await BeforeUpdateAsync(id, entity, localEntity);

                DbContext.Update(entity);
                await DbContext.SaveChangesAsync();
                await AfterUpdateAsync(entity);
            }

            return entity;
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null)
                return;

            if (await CanDeleteAsync(entity))
            {
                DbContext.Remove(entity);
                await DbContext.SaveChangesAsync();
            }
        }

        protected virtual Task<bool> CanDeleteAsync(TEntity entity)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> CanAddAsync(TEntity entity)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<bool> CanUpdateAsync(TEntity entity)
        {
            return Task.FromResult(true);
        }

        protected virtual Task<TEntity> BeforeUpdateAsync(Guid id, TEntity entity, TEntity localEntity)
        {
            return Task.FromResult(entity);
        }

        protected virtual Task<TEntity> BeforeAddAsync(TEntity entity)
        {
            return Task.FromResult(entity);
        }

        protected virtual Task<TEntity> AfterAddAsync(TEntity entity)
        {
            return Task.FromResult(entity);
        }

        protected virtual Task<TEntity> AfterUpdateAsync(TEntity entity)
        {
            return Task.FromResult(entity);
        }

        public virtual void DetachLocal<T>(Entity? entity, EntityState newState) where T : Entity
        {
            if (entity == null) return; ;
            var local = DbContext.Set<T>().Local.FirstOrDefault(x => x.Id == entity.Id);
            if (local != null)
            {
                var s = DbContext.Entry(local).State;
                DbContext.Entry(local).State = EntityState.Detached;
            }
            DbContext.Entry(entity).State = newState;
        }

    }
}
