// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.SeedWork.Entity
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Mediator;

namespace BuildingBlocks.SeedWork;

public abstract class Entity
{
    private int? _requestedHashCode;
    private int _Id;
    private List<INotification> _domainEvents;

    public virtual int Id
    {
        get => this._Id;
        set => this._Id = value;
    }

    public IReadOnlyCollection<INotification> DomainEvents
    {
        get
        {
            List<INotification> domainEvents = this._domainEvents;
            return (IReadOnlyCollection<INotification>)domainEvents?.AsReadOnly();
        }
    }

    public virtual bool IsTransientRecord() => this.Id == 0;

    public void AddDomainEvent(INotification eventItem)
    {
        this._domainEvents = this._domainEvents ?? new List<INotification>();
        this._domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(INotification eventItem) => this._domainEvents?.Remove(eventItem);

    public void ClearDomainEvents() => this._domainEvents?.Clear();

    public bool IsTransient() => this.Id == 0;

    public override bool Equals(object obj)
    {
        if (obj == null || (object)(obj as Entity) == null)
            return false;
        if ((object)this == obj)
            return true;
        if (this.GetType() != obj.GetType())
            return false;
        Entity entity = (Entity)obj;
        return !entity.IsTransient() && !this.IsTransient() && entity.Id == this.Id;
    }

    public override int GetHashCode()
    {
        if (this.IsTransient())
            return base.GetHashCode();
        if (!this._requestedHashCode.HasValue)
            this._requestedHashCode = new int?(this.Id.GetHashCode() ^ 31);
        return this._requestedHashCode.Value;
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (!object.Equals((object)left, (object)null))
            return left.Equals((object)right);
        return object.Equals((object)right, (object)null);
    }

    public static bool operator !=(Entity left, Entity right) => !(left == right);

    public Type GetUnproxiedType()
    {
        Type type = this.GetType();
        string str = type.ToString();
        return str.Contains("Castle.Proxies.") || str.Contains("System.Data.Entity.") || str.EndsWith("Proxy")
            ? type.BaseType
            : type;
    }
}