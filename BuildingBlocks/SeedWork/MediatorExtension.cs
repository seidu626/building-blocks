using Mediator;
using Microsoft.EntityFrameworkCore;

namespace BuildingBlocks.SeedWork
{
    public static class MediatorExtension
    {
        public static async ValueTask DispatchDomainEventsAsync<TDataContext>(
            this IMediator mediator,
            TDataContext context,
            CancellationToken cancellationToken)
            where TDataContext : DbContext
        {
            // Fetch entities with domain events
            var entitiesWithEvents = context.ChangeTracker
                .Entries<Entity>()
                .Where(entry => entry.Entity.DomainEvents != null && entry.Entity.DomainEvents.Any())
                .ToArray();

            // Collect all domain events
            var domainEvents = entitiesWithEvents
                .SelectMany(entry => entry.Entity.DomainEvents ?? [])
                .ToList();

            // Clear domain events from entities
            foreach (var entity in entitiesWithEvents)
            {
                entity.Entity.ClearDomainEvents();
            }

            // Publish each domain event asynchronously
            foreach (var domainEvent in domainEvents)
            {
                await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task DispatchDomainEventsAsync2<TContext>(this IMediator mediator, TContext ctx,
            CancellationToken cancellationToken) where TContext : DbContext
        {
            var domainEntities = ctx.ChangeTracker.Entries<Entity>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any()).ToArray();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // Clear domain events
            domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await mediator.Publish(domainEvent, cancellationToken).ConfigureAwait(false);
                });

            await Task.WhenAll(tasks);
        }
    }
}