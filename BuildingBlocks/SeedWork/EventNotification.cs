using Mediator;

namespace BuildingBlocks.SeedWork;

public class EventNotification<TDomainEvent> where TDomainEvent : INotification
{
    public EventNotification(TDomainEvent domainEvent) => this.DomainEvent = domainEvent;

    public TDomainEvent DomainEvent { get; }
}