using BuildingBlocks.Exceptions;
using BuildingBlocks.Extensions;
using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Persistence
{
    public static class RepositoryExtensions
    {
        public static IEnumerable<T> GetMany<T>(this IRepository<T> repository, IEnumerable<int> ids) where T : Entity
        {
            foreach (var idChunk in CollectionSlicer.Slice(ids, 128))
            {
                var entities = repository.Table.Where(e => idChunk.Contains(e.Id)).ToList();
                foreach (var entity in entities)
                {
                    yield return entity;
                }
            }
        }

        public static async Task<IEnumerable<T>> GetManyAsync<T>(this IRepository<T> repository, IEnumerable<int> ids)
            where T : Entity
        {
            var result = new List<T>();
            foreach (var idChunk in CollectionSlicer.Slice(ids, 128))
            {
                // Specify the correct ToListAsync method
                var entities =
                    await EntityFrameworkQueryableExtensions.ToListAsync(
                        repository.Table.Where(e => idChunk.Contains(e.Id)));
                result.AddRange(entities);
            }

            return result;
        }

        public static void Delete<T>(this IRepository<T> repository, int id) where T : Entity
        {
            Guard.AgainstZero(id, nameof(id));
            var entity = repository.Create();
            entity.Id = id;
            repository.ContextProvider.DbContext.Entry(entity).State =
                (Microsoft.EntityFrameworkCore.EntityState)EntityState.Deleted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DeleteRange<T>(this IRepository<T> repository, IEnumerable<int> ids) where T : Entity
        {
            Guard.AgainstNull(ids, nameof(ids));
            foreach (var id in ids)
            {
                repository.Delete(id);
            }
        }

        public static int DeleteAll<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate = null,
            bool cascade = false) where T : Entity
        {
            var entities = repository.Table;

            if (predicate != null)
            {
                entities = entities.Where(predicate);
            }

            var count = 0;

            if (cascade)
            {
                foreach (var chunk in CollectionSlicer.Slice(entities.ToList(), 500))
                {
                    repository.DeleteRange(chunk.Select(e => e.Id));
                    count += repository.ContextProvider.DbContext.SaveChanges();
                }
            }
            else
            {
                var entityIds = entities.Select(e => new { e.Id }).ToList();
                foreach (var chunk in CollectionSlicer.Slice(entityIds, 500))
                {
                    repository.DeleteRange(chunk.Select(x => x.Id));
                    count += repository.ContextProvider.DbContext.SaveChanges();
                }
            }

            return count;
        }

        public static async Task<int> DeleteAllAsync<T>(this IRepository<T> repository,
            Expression<Func<T, bool>> predicate = null, bool cascade = false) where T : Entity
        {
            var entities = repository.Table;

            if (predicate != null)
            {
                entities = entities.Where(predicate);
            }

            var count = 0;

            if (cascade)
            {
                foreach (var chunk in CollectionSlicer.Slice(
                             await EntityFrameworkQueryableExtensions.ToListAsync(entities), 500))
                {
                    repository.DeleteRange(chunk.Select(e => e.Id));
                    count += await repository.ContextProvider.DbContext.SaveChangesAsync();
                }
            }
            else
            {
                var entityIds =
                    await EntityFrameworkQueryableExtensions.ToListAsync(entities.Select(e => new { e.Id }));
                foreach (var chunk in CollectionSlicer.Slice(entityIds, 500))
                {
                    repository.DeleteRange(chunk.Select(x => x.Id));
                    count += await repository.ContextProvider.DbContext.SaveChangesAsync();
                }
            }

            return count;
        }

        public static IQueryable<T> Get<T>(this IRepository<T> repository, Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "") where T : Entity
        {
            var query = repository.Table;

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' },
                             StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty.Trim());
                }
            }

            return orderBy != null ? orderBy(query) : query;
        }
    }
}