using AutoMapper;
using BuildingBlocks.SeedWork;

#nullable disable
namespace BuildingBlocks.Persistence.DataStore;

public interface IDataStore<TKey, TDBModel, TModel>
    where TKey : IEquatable<TKey>
    where TDBModel : class, new()
    where TModel : IDTO, new()
{
    IUnitOfWork UnitOfWork { get; }
    IMapper Mapper { get; }

    Task<TModel> GetDTOByKeyAsync(TKey key, PropertyReference[] propertyReferences = null,
        CancellationToken cancellationToken = default);

    Task<TDBModel> GetByKeyAsync(TKey key, PropertyReference[] propertyReferences = null,
        CancellationToken cancellationToken = default);

    IQueryable<T> Query<T>() where T : class, new();
    IQueryable<TModel> Query();

    TKey ModelKey(TModel model);
    void SetModelKey(TModel model, TKey key);

    StoreResult Create(params TModel[] items);
    Task<StoreResult> CreateAsync(CancellationToken cancellationToken = default, params TModel[] items);

    StoreResult Update(params TModel[] items);
    Task<StoreResult> UpdateAsync(CancellationToken cancellationToken = default, params TModel[] items);

    StoreResult Delete(params TKey[] ids);
    Task<StoreResult> DeleteAsync(CancellationToken cancellationToken = default, params TKey[] ids);

    StoreResult Create(params TDBModel[] items);
    Task<StoreResult> CreateAsync(CancellationToken cancellationToken = default, params TDBModel[] items);

    StoreResult Update(params TDBModel[] items);
    Task<StoreResult> UpdateAsync(CancellationToken cancellationToken = default, params TDBModel[] items);
}