using AutoMapper;
using AutoMapper.QueryableExtensions;
using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace BuildingBlocks.Persistence.DataStore
{
    public abstract class EFDataStore<TEfContext, TKey, TDbModel, TModel> : IDataStore<TKey, TDbModel, TModel>
        where TEfContext : DbContext, new()
        where TKey : IEquatable<TKey>
        where TDbModel : class, new()
        where TModel : IDTO, new()
    {
        protected EFDataStore(TEfContext context, IMapper mapper)
        {
            Mapper = mapper;
            DbContext = context;
        }

        public TEfContext DbContext { get; }
        public abstract IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }

        public virtual IQueryable<TModel> Query()
        {
            return DbContext.Set<TDbModel>()
                .ProjectTo<TModel>(Mapper.ConfigurationProvider);
        }

        public virtual IQueryable<T> Query<T>() where T : class, new()
        {
            return DbContext.Set<TDbModel>()
                .ProjectTo<T>(Mapper.ConfigurationProvider);
        }

        public virtual async Task<TModel> GetDTOByKeyAsync(TKey key, PropertyReference[] propertyReferences = null,
            CancellationToken cancellationToken = default)
        {
            var dbModel = await EfGetByKeyAsync(key, propertyReferences, cancellationToken);
            return Mapper.Map<TDbModel, TModel>(dbModel);
        }

        public virtual async Task<TDbModel> GetByKeyAsync(TKey key, PropertyReference[] propertyReferences = null,
            CancellationToken cancellationToken = default)
        {
            return await EfGetByKeyAsync(key, propertyReferences, cancellationToken);
        }

        public abstract void SetModelKey(TModel model, TKey key);
        public abstract void SetModelKey(TDbModel model, TKey key);

        public abstract TKey ModelKey(TModel model);
        public abstract TKey ModelKey(TDbModel model);

        public virtual StoreResult Create(params TModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return TransactionalExec((s, t) =>
            {
                foreach (var model in items)
                {
                    var dbModel = Mapper.Map<TModel, TDbModel>(model);
                    var entityEntry = DbContext.Set<TDbModel>().Add(dbModel);
                    DbContext.SaveChanges();
                    SetModelKey(model, DbModelKey(entityEntry.Entity));
                }

                try
                {
                    DbContext.SaveChanges();
                    t.Commit();
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false);
        }

        public virtual async Task<StoreResult> CreateAsync(CancellationToken cancellationToken = default,
            params TModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return await TransactionalExecAsync(async (s, t) =>
            {
                foreach (var item in items)
                {
                    var dbModel = Mapper.Map<TModel, TDbModel>(item);
                    var entityEntry = DbContext.Set<TDbModel>().Add(dbModel);
                    await DbContext.SaveChangesAsync(cancellationToken);
                    SetModelKey(item, DbModelKey(entityEntry.Entity));
                }

                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken);
                    await t.CommitAsync(cancellationToken);
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false, cancellationToken);
        }

        public virtual StoreResult Update(params TModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return TransactionalExec((s, t) =>
            {
                foreach (var model in items)
                {
                    var key = ModelKey(model);
                    var dbModel = EfGetByKeyAsync(key).Result;
                    if (dbModel == null)
                    {
                        return new StoreResult
                        {
                            Success = false, Error = $"Unable to locate {typeof(TDbModel).Name}({key}) in datastore"
                        };
                    }

                    Mapper.Map(model, dbModel);
                    DbContext.Entry(dbModel).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    DbContext.SaveChanges();
                }

                try
                {
                    DbContext.SaveChanges();
                    t.Commit();
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false);
        }

        public virtual async Task<StoreResult> UpdateAsync(CancellationToken cancellationToken = default,
            params TModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return await TransactionalExecAsync(async (s, t) =>
            {
                foreach (var item in items)
                {
                    var key = ModelKey(item);
                    var dbModel = await EfGetByKeyAsync(key, cancellationToken: cancellationToken);
                    if (dbModel == null)
                    {
                        return new StoreResult
                        {
                            Success = false, Error = $"Unable to locate {typeof(TDbModel).Name}({key}) in datastore"
                        };
                    }

                    Mapper.Map(item, dbModel);
                    DbContext.Entry(dbModel).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    await DbContext.SaveChangesAsync(cancellationToken);
                }

                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken);
                    await t.CommitAsync(cancellationToken);
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false, cancellationToken);
        }

        public virtual StoreResult Create(params TDbModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return TransactionalExec((s, t) =>
            {
                foreach (var model in items)
                {
                    var entityEntry = DbContext.Set<TDbModel>().Add(model);
                    DbContext.SaveChanges();
                    SetModelKey(model, DbModelKey(entityEntry.Entity));
                }

                try
                {
                    DbContext.SaveChanges();
                    t.Commit();
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false);
        }

        public virtual async Task<StoreResult> CreateAsync(CancellationToken cancellationToken = default,
            params TDbModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return await TransactionalExecAsync(async (s, t) =>
            {
                foreach (var item in items)
                {
                    var entityEntry = DbContext.Set<TDbModel>().Add(item);
                    await DbContext.SaveChangesAsync(cancellationToken);
                    SetModelKey(item, DbModelKey(entityEntry.Entity));
                }

                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken);
                    await t.CommitAsync(cancellationToken);
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false, cancellationToken);
        }

        public virtual StoreResult Update(params TDbModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return TransactionalExec((s, t) =>
            {
                foreach (var model in items)
                {
                    var dbModel = DbContext.Set<TDbModel>().Find(DbModelKey(model));
                    if (dbModel == null)
                    {
                        return new StoreResult
                            { Success = false, Error = $"Unable to locate {typeof(TDbModel).Name} in datastore" };
                    }

                    Mapper.Map(model, dbModel);
                    DbContext.Entry(dbModel).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    DbContext.SaveChanges();
                }

                try
                {
                    DbContext.SaveChanges();
                    t.Commit();
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false);
        }

        public virtual async Task<StoreResult> UpdateAsync(CancellationToken cancellationToken = default,
            params TDbModel[] items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            return await TransactionalExecAsync(async (s, t) =>
            {
                foreach (var item in items)
                {
                    var dbModel = await DbContext.Set<TDbModel>()
                        .FindAsync(new object[] { DbModelKey(item) }, cancellationToken);
                    if (dbModel == null)
                    {
                        return new StoreResult
                            { Success = false, Error = $"Unable to locate {typeof(TDbModel).Name} in datastore" };
                    }

                    Mapper.Map(item, dbModel);
                    DbContext.Entry(dbModel).State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
                    await DbContext.SaveChangesAsync(cancellationToken);
                }

                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken);
                    await t.CommitAsync(cancellationToken);
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false, cancellationToken);
        }

        public virtual StoreResult Delete(params TKey[] ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            return TransactionalExec((s, t) =>
            {
                foreach (var key in ids)
                {
                    var dbModel = EfGetByKeyAsync(key).Result;
                    if (dbModel != null)
                    {
                        DbContext.Set<TDbModel>().Remove(dbModel);
                    }
                }

                try
                {
                    DbContext.SaveChanges();
                    t.Commit();
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false);
        }

        public virtual async Task<StoreResult> DeleteAsync(CancellationToken cancellationToken = default,
            params TKey[] ids)
        {
            if (ids == null) throw new ArgumentNullException(nameof(ids));

            return await TransactionalExecAsync(async (s, t) =>
            {
                foreach (var key in ids)
                {
                    var dbModel = await EfGetByKeyAsync(key, cancellationToken: cancellationToken);
                    if (dbModel != null)
                    {
                        DbContext.Set<TDbModel>().Remove(dbModel);
                    }
                }

                try
                {
                    await DbContext.SaveChangesAsync(cancellationToken);
                    await t.CommitAsync(cancellationToken);
                    return new StoreResult { Success = true };
                }
                catch (Exception ex)
                {
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }, false, cancellationToken);
        }

        protected async Task<TDbModel> EfGetByKeyAsync(TKey key, PropertyReference[] propertyReferences = null,
            CancellationToken cancellationToken = default)
        {
            var query = DbContext.Set<TDbModel>().AsQueryable();

            if (propertyReferences != null)
            {
                foreach (var propRef in propertyReferences)
                {
                    query = query.Include(propRef.PropertyName);
                }
            }

            return await query.FirstOrDefaultAsync(entity => EF.Property<TKey>(entity, "Id").Equals(key),
                cancellationToken);
        }

        protected virtual StoreResult TransactionalExec(Func<DbContext, IDbContextTransaction, StoreResult> action,
            bool suppressSaveChanges)
        {
            using (var transaction = DbContext.Database.BeginTransaction())
            {
                try
                {
                    var result = action(DbContext, transaction);
                    if (result.Success && !suppressSaveChanges)
                    {
                        DbContext.SaveChanges();
                    }

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }
        }

        protected virtual async Task<StoreResult> TransactionalExecAsync(
            Func<DbContext, IDbContextTransaction, Task<StoreResult>> action, bool suppressSaveChanges,
            CancellationToken cancellationToken)
        {
            using (var transaction = await DbContext.Database.BeginTransactionAsync(cancellationToken))
            {
                try
                {
                    var result = await action(DbContext, transaction);
                    if (result.Success && !suppressSaveChanges)
                    {
                        await DbContext.SaveChangesAsync(cancellationToken);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new StoreResult { Success = false, Error = ex.InnerException?.Message ?? ex.Message };
                }
            }
        }

        private TKey DbModelKey(TDbModel model)
        {
            // Implementation for extracting the key from TDbModel. Assumes model is not null.
            var keyProperty = DbContext.Model.FindEntityType(typeof(TDbModel)).FindPrimaryKey().Properties[0];
            return (TKey)model.GetType().GetProperty(keyProperty.Name).GetValue(model);
        }
    }

    public class StoreResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
    }

    public interface IDTO
    {
    }

    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class PropertyReference
    {
        public string PropertyName { get; set; }
    }
}