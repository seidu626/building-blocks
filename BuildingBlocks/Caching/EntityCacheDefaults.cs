using BuildingBlocks.SeedWork;

namespace BuildingBlocks.Caching
{
    public static class EntityCacheDefaults<TEntity> where TEntity : Entity
    {
        public static string EntityTypeName => typeof(TEntity).Name.ToLowerInvariant();

        public static CacheKey ByIdCacheKey => new CacheKey(
            $"App.{EntityTypeName}.byid.{{0}}",
            new[] { ByIdPrefix, Prefix });

        public static CacheKey ByIdsCacheKey => new CacheKey(
            $"App.{EntityTypeName}.byids.{{0}}",
            new[] { ByIdsPrefix, Prefix });

        public static CacheKey AllCacheKey => new CacheKey(
            $"App.{EntityTypeName}.all.",
            new[] { AllPrefix, Prefix });

        public static string Prefix => $"App.{EntityTypeName}.";

        public static string ByIdPrefix => $"App.{EntityTypeName}.byid.";

        public static string ByIdsPrefix => $"App.{EntityTypeName}.byids.";

        public static string AllPrefix => $"App.{EntityTypeName}.all.";
    }
}