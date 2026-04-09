namespace BuildingBlocks.SeedWork;

public abstract class TrackableEntity<TId> : Entity<TId>, ITrackable
{
    public DateTime CreatedAt { get; private set; }
    public DateTime LastUpdatedAt { get; private set; }
    public string CreatedBy { get; private set; }
    public string LastUpdatedBy { get; private set; }
        
    protected TrackableEntity()
    {
        var now = DateTime.UtcNow;
        CreatedAt = now;
        LastUpdatedAt = now;
    }
        
    public void SetCreationTracking(string userId, DateTime timestamp)
    {
        CreatedAt = timestamp;
        LastUpdatedAt = timestamp;
        CreatedBy = userId;
        LastUpdatedBy = userId;
    }
        
    public void UpdateTracking(string userId)
    {
        LastUpdatedAt = DateTime.UtcNow;
        LastUpdatedBy = userId;
    }
}
