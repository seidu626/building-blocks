using BuildingBlocks.SeedWork;
using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.Extensions
{
    public static class MediatorExtensions
    {
        public static async Task DispatchDomainEventsAsync<TContext>(this IMediator mediator, TContext ctx, CancellationToken cancellationToken) where TContext : DbContext
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToArray();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // Clear domain events
            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) => {
                    await mediator.Publish(domainEvent, cancellationToken);
                });

            await Task.WhenAll(tasks);
        }
    }
}