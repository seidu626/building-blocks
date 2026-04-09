using BuildingBlocks.Extensions;
using Mediator;

namespace BuildingBlocks.SeedWork
{
    public abstract class Entity<TId>
    {
        private int? _requestedHashCode;
        private TId _id;
        private List<INotification>? _domainEvents;

        public virtual TId Id
        {
            get => _id;
            set => _id = value;
        }

        public IReadOnlyCollection<INotification>? DomainEvents =>
            _domainEvents?.AsReadOnly();

        public virtual bool IsTransient() => Id.IsDefault();

        public void AddDomainEvent(INotification eventItem)
        {
            _domainEvents ??= new List<INotification>();
            _domainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem) =>
            _domainEvents?.Remove(eventItem);

        public void ClearDomainEvents() =>
            _domainEvents?.Clear();

        public async ValueTask PublishDomainEvents(
            IDomainEventDispatcher domainEventDispatcher,
            CancellationToken cancellationToken)
        {
            if (_domainEvents == null || _domainEvents.Count == 0)
                return;

            await domainEventDispatcher.Dispatch(_domainEvents, cancellationToken);
            ClearDomainEvents();
        }

        public virtual bool IsTransientRecord() => this.Id.IsDefault();

        public override bool Equals(object? obj)
        {
            if (obj == null || obj is not Entity<TId> other)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (GetType() != obj.GetType())
                return false;

            return !IsTransient() && !other.IsTransient() && Id != null && Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            if (IsTransient())
                return base.GetHashCode();

            _requestedHashCode ??= Id.GetHashCode() ^ 31;
            return _requestedHashCode.Value;
        }

        public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
            Equals(left, right);

        public static bool operator !=(Entity<TId>? left, Entity<TId>? right) =>
            !(left == right);

        public Type GetUnproxiedType()
        {
            var type = GetType();
            var typeName = type.ToString();

            return typeName.Contains("Castle.Proxies.") ||
                   typeName.Contains("System.Data.Entity.") ||
                   typeName.EndsWith("Proxy")
                ? type.BaseType ?? type
                : type;
        }
    }
}