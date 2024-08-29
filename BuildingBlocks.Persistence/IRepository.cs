using BuildingBlocks.Caching;
using BuildingBlocks.SeedWork;
using BuildingBlocks.Types;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BuildingBlocks.Persistence
{
    public interface IRepository<TEntity> where TEntity : Entity
    {
        DbContextProvider ContextProvider { get; }
        IQueryable<TEntity> Table { get; }

        IQueryable<TEntity> TableNoTracking { get; }

        ICollection<TEntity> Local { get; }

        TEntity Create();

        Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null,
            Func<IStaticCacheManager, CacheKey> getCacheKey = null,
            bool includeDeleted = false);

        Task<IPagedList<TEntity>> GetPagedAsync(
            Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null,
            int pageIndex = 0,
            int pageSize = int.MaxValue,
            bool getOnlyTotalCount = false,
            bool includeDeleted = false);

        TEntity GetById(int id);

        ValueTask<TEntity> GetByIdAsync(
            int id,
            Func<IStaticCacheManager, CacheKey> getCacheKey = null,
            bool includeDeleted = false);

        Task<List<TEntity>> GetByIdsAsync(
            IEnumerable<int> ids,
            Func<IStaticCacheManager, CacheKey> getCacheKey = null,
            bool includeDeleted = false);

        EntityEntry<TEntity> Attach(TEntity entity);

        void Insert(TEntity entity);

        Task InsertAsync(TEntity entity);

        void InsertRange(IEnumerable<TEntity> entities);

        Task InsertRangeAsync(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        Task UpdateAsync(TEntity entity);

        void UpdateRange(IEnumerable<TEntity> entities);

        Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        Task DeleteAsync(TEntity entity);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        void DeleteRange(IEnumerable<TEntity> entities);

        Task DeleteRangeAsync(IEnumerable<TEntity> entities);

        Task TruncateAsync(string tableName);
    }
}