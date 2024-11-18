using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBuildingBlocks(this IServiceCollection services)
        {
            return services;
        }
    }
}