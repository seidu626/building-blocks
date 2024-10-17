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
            Func<ICacheKeyService, CacheKey> getCacheKey = null,
            bool includeDeleted = false);
        
        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true);

        /// <summary>
        /// Get all entity entries
        /// </summary>
        /// <param name="func">Function to select entries</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="getOnlyTotalCount">Whether to get only the total number of entries without actually loading data</param>
        /// <param name="includeDeleted">Whether to include deleted items (applies only to <see cref="Nop.Core.Domain.Common.ISoftDeletedEntity"/> entities)</param>
        /// <returns>
        /// A task that represents the asynchronous operation
        /// The task result contains the paged list of entity entries
        /// </returns>
        Task<IPagedList<TEntity>> GetAllPagedAsync(Func<IQueryable<TEntity>, Task<IQueryable<TEntity>>> func = null,
            int pageIndex = 0, int pageSize = int.MaxValue, bool getOnlyTotalCount = false, bool includeDeleted = true);


        TEntity GetById(int id);

        ValueTask<TEntity> GetByIdAsync(
            int id,
            Func<ICacheKeyService, CacheKey> getCacheKey = null,
            bool includeDeleted = false);

        Task<List<TEntity>> GetByIdsAsync(
            IEnumerable<int> ids,
            Func<ICacheKeyService, CacheKey> getCacheKey = null,
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