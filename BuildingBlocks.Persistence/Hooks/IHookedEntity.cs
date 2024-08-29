using BuildingBlocks.SeedWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;

namespace BuildingBlocks.Persistence.Hooks
{
    public interface IHookedEntity<TDbContext>
    {
        Type ContextType { get; }

        EntityEntry Entry { get; }

        Entity Entity { get; }

        Type EntityType { get; }

        EntityState InitialState { get; set; }

        EntityState State { get; set; }

        bool HasStateChanged { get; }

        bool IsPropertyModified(string propertyName);

        bool IsSoftDeleted { get; }
    }
}