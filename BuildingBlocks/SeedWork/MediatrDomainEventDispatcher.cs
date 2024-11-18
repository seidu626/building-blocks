using Mediator;

namespace BuildingBlocks.SeedWork;

public class MediatrDomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public MediatrDomainEventDispatcher(IMediator mediator) => this._mediator = mediator;

    public ValueTask Dispatch(
        IEnumerable<INotification> domainEvents,
        CancellationToken cancellationToken)
    {
        Parallel.ForEach<INotification>(domainEvents, (Action<INotification>) (domainEvent => this._mediator.Publish<INotification>(domainEvent, cancellationToken)));
        return ValueTask.CompletedTask;
    }
}
