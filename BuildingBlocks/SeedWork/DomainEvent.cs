namespace BuildingBlocks.SeedWork;

public class DomainEvent
{
    public Guid Id { get; }
    public DateTime OccurredOn { get; }
        
    protected DomainEvent()
    {
        Id = Guid.NewGuid();
        OccurredOn = DateTime.UtcNow;
    }
}