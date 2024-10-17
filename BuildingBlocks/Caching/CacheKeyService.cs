using System.Globalization;
using System.Text;
using BuildingBlocks.Common;
using BuildingBlocks.Configuration;
using BuildingBlocks.SeedWork;

namespace BuildingBlocks.Caching
{
    public abstract class CacheKeyService :  ICacheKeyService
    {
        private readonly CacheConfig _cacheConfig;

        private string HashAlgorithm => "SHA1";

        protected CacheKeyService(CacheConfig cacheConfig)
        {
            _cacheConfig = cacheConfig;
        }

        protected virtual string PrepareKeyPrefix(string prefix, params object[] prefixParameters)
        {
            if (prefixParameters == null || !prefixParameters.Any())
                return prefix;

            var formattedParameters = prefixParameters.Select(CreateCacheKeyParameters).ToArray();
            return string.Format(prefix, formattedParameters);
        }

        protected virtual string CreateIdsHash(IEnumerable<int> ids)
        {
            var orderedIds = ids.OrderBy(id => id).ToList();
            return !orderedIds.Any() ? string.Empty : HashHelper.CreateHash(Encoding.UTF8.GetBytes(string.Join(", ", orderedIds)), HashAlgorithm);
        }

        protected virtual object CreateCacheKeyParameters(object parameter)
        {
            switch (parameter)
            {
                case null:
                    return "null";
                case IEnumerable<int> ids:
                    return CreateIdsHash(ids);
                case IEnumerable<Entity> entities:
                    return CreateIdsHash(entities.Select(entity => entity.Id));
                case Entity entity:
                    return entity.Id;
                case decimal num:
                    return num.ToString(CultureInfo.InvariantCulture);
                default:
                    return parameter;
            }
        }

        public virtual CacheKey PrepareKey(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            return cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
        }

        public virtual CacheKey PrepareKeyForDefaultCache(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var preparedKey = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
            preparedKey.CacheTime = _cacheConfig.DefaultCacheTime;
            return preparedKey;
        }

        public virtual CacheKey PrepareKeyForShortTermCache(CacheKey cacheKey, params object[] cacheKeyParameters)
        {
            var preparedKey = cacheKey.Create(CreateCacheKeyParameters, cacheKeyParameters);
            preparedKey.CacheTime = _cacheConfig.ShortTermCacheTime;
            return preparedKey;
        }
    }
}
