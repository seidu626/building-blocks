#nullable disable
using Mediator;

namespace BuildingBlocks.SeedWork
{
    public abstract class Entity
    {
        private int? _requestedHashCode;
        private int _id;
        private List<INotification> _domainEvents;

        public virtual int Id
        {
            get => _id;
            set => _id = value;
        }

        public IReadOnlyCollection<INotification> DomainEvents => _domainEvents?.AsReadOnly();

        public virtual bool IsTransientRecord() => Id == 0;

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) => _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() => _domainEvents?.Clear();

        public bool IsTransient() => Id == 0;

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity entity))
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            return GetType() == obj.GetType() && !IsTransient() && !entity.IsTransient() && Id == entity.Id;
        }

        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity left, Entity right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(Entity left, Entity right) => !(left == right);

        public Type GetUnproxiedType()
        {
            Type type = GetType();
            string typeName = type.ToString();

            if (typeName.Contains("Castle.Proxies.") || typeName.Contains("System.Data.Entity.") || typeName.EndsWith("Proxy"))
                return type.BaseType;

            return type;
        }
    }
}
