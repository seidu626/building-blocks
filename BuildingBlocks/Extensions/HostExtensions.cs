using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Extensions
{
    public static class HostExtensions
    {
        /// <summary>
        /// Applies any pending migrations for the context to the database.
        /// </summary>
        /// <typeparam name="TContext">The DbContext type.</typeparam>
        /// <param name="host">The IHost instance.</param>
        /// <returns>The IHost instance.</returns>
        public static IHost MigrateDatabase<TContext>(this IHost host) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<TContext>>();
                var context = services.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation("Applying migrations for context {DbContextName}", typeof(TContext).Name);
                    context.Database.Migrate();
                    logger.LogInformation("Migrations applied successfully for context {DbContextName}",
                        typeof(TContext).Name);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while applying migrations for context {DbContextName}",
                        typeof(TContext).Name);
                }
            }

            return host;
        }
    }
}