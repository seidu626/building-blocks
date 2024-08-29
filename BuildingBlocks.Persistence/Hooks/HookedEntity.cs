using BuildingBlocks.Exceptions;
using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace BuildingBlocks.Persistence.Hooks
{
    public class HookedEntity<TDbContext> : IHookedEntity<TDbContext> where TDbContext : DbContext
    {
        private Type _entityType;

        public HookedEntity(ObjectContextBase<TDbContext> contextProvider, EntityEntry entry)
            : this(contextProvider?.GetType(), entry)
        {
        }

        internal HookedEntity(Type contextType, EntityEntry entry)
        {
            ContextType = contextType ?? throw new ArgumentNullException(nameof(contextType));
            Entry = entry ?? throw new ArgumentNullException(nameof(entry));
            InitialState = (EntityState)entry.State;
        }

        public Type ContextType { get; }

        public EntityEntry Entry { get; }

        public Entity Entity => Entry.Entity as Entity;

        public Type EntityType
        {
            get
            {
                return _entityType ??= Entity?.GetUnproxiedType();
            }
        }

        public EntityState InitialState { get; set; }

        public EntityState State
        {
            get => (EntityState)Entry.State;
            set => Entry.State = (Microsoft.EntityFrameworkCore.EntityState)value;
        }

        public bool HasStateChanged => InitialState != State;

        public bool IsPropertyModified(string propertyName)
        {
            Guard.AgainstNull(propertyName, nameof(propertyName));

            if (State != EntityState.Modified)
                return false;

            var propertyEntry = Entry.Property(propertyName);
            if (propertyEntry == null)
                throw new DomainException($"An entity property '{propertyName}' does not exist.");

            return propertyEntry.CurrentValue != null && !propertyEntry.CurrentValue.Equals(propertyEntry.OriginalValue);
        }

        public bool IsSoftDeleted
        {
            get
            {
                if (Entry.Entity is not ISoftDeletedEntity entity)
                    return false;

                if (Entry.State != (Microsoft.EntityFrameworkCore.EntityState)EntityState.Deleted)
                    return entity.Deleted;

                return entity.Deleted && IsPropertyModified(nameof(ISoftDeletedEntity.Deleted));
            }
        }
    }
}
