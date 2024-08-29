using BuildingBlocks.Caching;
using BuildingBlocks.Events;
using BuildingBlocks.Exceptions;
using BuildingBlocks.SeedWork;
using BuildingBlocks.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Persistence
{
    public sealed class EntityRepository<TEntity> : IRepository<TEntity> where TEntity : Entity, new()
    {
        private readonly IEventPublisher _eventPublisher;
        private readonly IStaticCacheManager _staticCacheManager;
        private readonly DbContext _context;
        private readonly DbContextProvider _dbContextProvider;
        private DbSet<TEntity> _entities;

        public EntityRepository(
            IEventPublisher eventPublisher,
            IStaticCacheManager staticCacheManager,
            DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider ?? throw new ArgumentNullException(nameof(dbContextProvider));
            _context = _dbContextProvider.DbContext ?? throw new ArgumentNullException(nameof(_context));
            _eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
            _staticCacheManager = staticCacheManager ?? throw new ArgumentNullException(nameof(staticCacheManager));
        }

        public IQueryable<TEntity> Table => Entities;
        public IQueryable<TEntity> TableNoTracking { get; }

        public IQueryable<TEntity> TableUntracked => Entities.AsNoTracking();

        public ICollection<TEntity> Local => Entities.Local;
        public TEntity Create() => new TEntity();

        public Task<IList<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null, Func<IStaticCacheManager, CacheKey> getCacheKey = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public Task<IPagedList<TEntity>> GetPagedAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> predicate = null, int pageIndex = 0, int pageSize = Int32.MaxValue,
            bool getOnlyTotalCount = false, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<TEntity> GetByIdAsync(int id, Func<IStaticCacheManager, CacheKey> getCacheKey = null, bool includeDeleted = true)
        {
            if (getCacheKey == null)
            {
                return await GetEntityAsync();
            }

            var cacheKey = getCacheKey(_staticCacheManager) ?? _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<TEntity>.ByIdCacheKey, id);
            return await _staticCacheManager.GetAsync(cacheKey, GetEntityAsync);

            async Task<TEntity> GetEntityAsync()
            {
                return await AddDeletedFilter(Table, includeDeleted).FirstOrDefaultAsync(e => e.Id == id);
            }
        }

        public Task<List<TEntity>> GetByIdsAsync(IEnumerable<int> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null, bool includeDeleted = false)
        {
            throw new NotImplementedException();
        }

        public async Task<IList<TEntity>> GetByIdsAsync(IList<int> ids, Func<IStaticCacheManager, CacheKey> getCacheKey = null, bool includeDeleted = true)
        {
            if (ids == null || !ids.Any())
            {
                return new List<TEntity>();
            }

            if (getCacheKey == null)
            {
                return await GetEntitiesByIdsAsync();
            }

            var cacheKey = getCacheKey(_staticCacheManager) ?? _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<TEntity>.ByIdsCacheKey, ids);
            return await _staticCacheManager.GetAsync(cacheKey, GetEntitiesByIdsAsync);

            async Task<IList<TEntity>> GetEntitiesByIdsAsync()
            {
                var query = AddDeletedFilter(Table, includeDeleted).Where(e => ids.Contains(e.Id));
                var entities = await query.ToListAsync();
                return ids.Select(id => entities.FirstOrDefault(e => e.Id == id)).Where(e => e != null).ToList();
            }
        }

        public async Task InsertAsync(TEntity entity, bool publishEvent = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            await _context.AddAsync(entity);
            if (publishEvent)
            {
                await _eventPublisher.EntityInsertedAsync(entity);
            }
        }

        public async Task InsertAsync(IList<TEntity> entities, bool publishEvent = true)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentNullException(nameof(entities));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.AddRangeAsync(entities);
                await transaction.CommitAsync();

                if (publishEvent)
                {
                    foreach (var entity in entities)
                    {
                        await _eventPublisher.EntityInsertedAsync(entity);
                    }
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<TEntity> LoadOriginalCopyAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            return await _context.FindAsync<TEntity>(entity.Id);
        }

        public async Task UpdateAsync(TEntity entity, bool publishEvent = true)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Update(entity);
            if (publishEvent)
            {
                await _eventPublisher.EntityUpdatedAsync(entity);
            }
        }

        public async Task UpdateAsync(IList<TEntity> entities, bool publishEvent = true)
        {
            if (entities == null || !entities.Any()) throw new ArgumentNullException(nameof(entities));

            _context.UpdateRange(entities);
            if (publishEvent)
            {
                foreach (var entity in entities)
                {
                    await _eventPublisher.EntityUpdatedAsync(entity);
                }
            }
        }

        public async Task DeleteAsync(TEntity entity, bool publishEvent = true)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            if (entity is ISoftDeletedEntity softDeletedEntity)
            {
                softDeletedEntity.Deleted = true;
                _context.Update(entity);
            }
            else
            {
                _context.Remove(entity);
            }

            if (publishEvent)
            {
                await _eventPublisher.EntityDeletedAsync(entity);
            }
        }

        public async Task DeleteAsync(IList<TEntity> entities, bool publishEvent = true)
        {
            if (entities == null || !entities.Any()) throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                if (entity is ISoftDeletedEntity softDeletedEntity)
                {
                    softDeletedEntity.Deleted = true;
                    _context.Update(entity);
                }
                else
                {
                    _context.Remove(entity);
                }
            }

            if (publishEvent)
            {
                foreach (var entity in entities)
                {
                    await _eventPublisher.EntityDeletedAsync(entity);
                }
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var entities = await Entities.Where(predicate).ToListAsync();
            _context.RemoveRange(entities);
        }

        public async Task TruncateAsync(string tableName)
        {
            if (string.IsNullOrEmpty(tableName)) throw new ArgumentNullException(nameof(tableName));

            await _context.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE {tableName}");
        }

        public TEntity GetById(int id)
        {
            return Entities.Find(id);
        }

        public EntityEntry<TEntity> Attach(TEntity entity)
        {
            return Entities.Attach(entity);
        }

        public void Insert(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            Entities.Add(entity);
        }

        public async Task InsertAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            await Entities.AddAsync(entity);
        }

        public void InsertRange(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            throw new NotImplementedException();
        }

        public void InsertRange(IEnumerable<TEntity> entities, int batchSize = 100)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            Entities.AddRange(entities);
        }

        public async Task InsertRangeAsync(IEnumerable<TEntity> entities, int batchSize = 100)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            await Entities.AddRangeAsync(entities);
        }

        public void Update(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            ChangeStateToModifiedIfApplicable(entity);
        }

        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            ChangeStateToModifiedIfApplicable(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                ChangeStateToModifiedIfApplicable(entity);
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            foreach (var entity in entities)
            {
                ChangeStateToModifiedIfApplicable(entity);
            }
        }

        public void Delete(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Deleted;
        }

        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            _context.Entry(entity).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Deleted;
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            _context.RemoveRange(entities);
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            _context.RemoveRange(entities);
        }

        public DbContextProvider ContextProvider => _dbContextProvider;

        private async Task<IList<TEntity>> GetEntitiesAsync(Func<Task<IList<TEntity>>> getAllAsync, Func<IStaticCacheManager, CacheKey> getCacheKey)
        {
            return getCacheKey == null
                ? await getAllAsync()
                : await _staticCacheManager.GetAsync(getCacheKey(_staticCacheManager) ?? _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<TEntity>.AllCacheKey, Array.Empty<object>()), getAllAsync);
        }

        private IList<TEntity> GetEntities(Func<IList<TEntity>> getAll, Func<IStaticCacheManager, CacheKey> getCacheKey)
        {
            return getCacheKey == null
                ? getAll()
                : _staticCacheManager.Get(getCacheKey(_staticCacheManager) ?? _staticCacheManager.PrepareKeyForDefaultCache(EntityCacheDefaults<TEntity>.AllCacheKey, Array.Empty<object>()), getAll);
        }

        private IQueryable<TEntity> AddDeletedFilter(IQueryable<TEntity> query, bool includeDeleted)
        {
            if (includeDeleted || typeof(ISoftDeletedEntity).IsAssignableFrom(typeof(TEntity)) == false)
            {
                return query;
            }

            return query.OfType<ISoftDeletedEntity>()
                        .Where(e => !e.Deleted)
                        .OfType<TEntity>();
        }

        private void ChangeStateToModifiedIfApplicable(TEntity entity)
        {
            if (!entity.IsTransientRecord())
            {
                _context.Entry(entity).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
            }
        }

        private DbSet<TEntity> Entities => _entities ??= _context.Set<TEntity>();
    }
}
