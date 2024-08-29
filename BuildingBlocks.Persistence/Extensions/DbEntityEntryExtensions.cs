using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildingBlocks.Persistence.Extensions
{
    public static class DbEntityEntryExtensions
    {
        /// <summary>
        /// Reloads the entity from the database.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entity">The entity instance.</param>
        public static void ReloadEntity<TEntity>(this DbContext context, TEntity entity) where TEntity : class
        {
            context.Entry(entity).Reload();
        }

        /// <summary>
        /// Reloads the entity from the database and updates its state.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <param name="entry">The EntityEntry instance.</param>
        /// <returns>The reloaded entity.</returns>
        public static TEntity ReloadEntity<TEntity>(this EntityEntry<TEntity> entry) where TEntity : class
        {
            if (entry.State == (Microsoft.EntityFrameworkCore.EntityState)EntityState.Detached)
                return entry.Entity;

            DbContext context = entry.Context;
            object[] entityKey = context.GetEntityKey(entry.Entity);

            entry.State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Detached;
            TEntity entity = context.Set<TEntity>().Find(entityKey);

            if (entity == null)
                throw new InvalidOperationException("Entity not found.");

            EntityEntry<TEntity> entityEntry = context.Entry(entity);

            foreach (var property in entityEntry.Properties)
                property.CurrentValue = entry.Property(property.Metadata.Name).CurrentValue;

            entityEntry.State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Unchanged;
            entry.State = (Microsoft.EntityFrameworkCore.EntityState)EntityState.Unchanged;

            return entry.Entity;
        }

        /// <summary>
        /// Gets the entity's primary key values.
        /// </summary>
        /// <typeparam name="T">The type of the entity.</typeparam>
        /// <param name="context">The DbContext instance.</param>
        /// <param name="entity">The entity instance.</param>
        /// <returns>An array of primary key values.</returns>
        public static object[] GetEntityKey<T>(this DbContext context, T entity) where T : class
        {
            var keyProperties = context.Entry(entity).Metadata.FindPrimaryKey().Properties;
            return keyProperties.Select(p => p.GetGetter().GetClrValue(entity)).ToArray();
        }

        /// <summary>
        /// Determines if the entity has changes.
        /// </summary>
        /// <param name="entry">The EntityEntry instance.</param>
        /// <returns>True if the entity has changes; otherwise, false.</returns>
        public static bool HasChanges(this EntityEntry entry)
        {
            return entry.State == (Microsoft.EntityFrameworkCore.EntityState)EntityState.Modified;
        }

        /// <summary>
        /// Compares two objects for equality, handling nulls.
        /// </summary>
        /// <param name="cur">The current object.</param>
        /// <param name="orig">The original object.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        private static bool AreEqual(object cur, object orig)
        {
            return cur == null ? orig == null : cur.Equals(orig);
        }
    }
}
