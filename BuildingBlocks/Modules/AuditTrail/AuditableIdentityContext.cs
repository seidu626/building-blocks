#nullable enable
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace BuildingBlocks.Modules.AuditTrail;

public abstract class AuditableIdentityContext : DbContext
{
    private readonly Serilog.ILogger _logger = Serilog.Log.Logger;

    protected AuditableIdentityContext()
    {
    }

    protected AuditableIdentityContext(
#nullable disable
        DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<Audit> AuditLogs { get; set; }

    protected virtual async Task<int> SaveChangesAsync(string username,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        try
        {
            this.OnBeforeSaveChanges(username);
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            // Log each entry that caused the exception
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                                     e.State == EntityState.Modified ||
                                                                     e.State == EntityState.Deleted))
            {
                var entityType = entry.Entity.GetType().Name;
                var state = entry.State;
                var keyValues = entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

                _logger.Error("Failed to save entity {Entity} of type {EntityType}. State: {State}. Keys: {Keys}",
                    JsonConvert.SerializeObject(entry.Entity), entityType, state, keyValues);
            }

            var dEx = dbEx.Demystify();
            _logger.Error(dEx, dEx.Message);
            // Re-throw the exception to ensure proper error handling
            throw;
        }
        catch (Exception ex)
        {
            var dEx = ex.Demystify();
            _logger.Error(dEx, "An unexpected error occurred while saving changes.");
            throw;
        }
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        catch (DbUpdateException dbEx)
        {
            // Log each entry that caused the exception
            foreach (var entry in ChangeTracker.Entries().Where(e => e.State == EntityState.Added ||
                                                                     e.State == EntityState.Modified ||
                                                                     e.State == EntityState.Deleted))
            {
                var entityType = entry.Entity.GetType().Name;
                var state = entry.State;
                var keyValues = entry.Properties
                    .Where(p => p.Metadata.IsPrimaryKey())
                    .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

                _logger.Error("Failed to save entity {Entity} of type {EntityType}. State: {State}. Keys: {Keys}",
                    JsonConvert.SerializeObject(entry.Entity), entityType, state, keyValues);
            }

            var dEx = dbEx.Demystify();
            _logger.Error(dEx, dEx.Message);
            // Re-throw the exception to ensure proper error handling
            throw;
        }
        catch (Exception ex)
        {
            var dEx = ex.Demystify();
            _logger.Error(dEx, "An unexpected error occurred while saving changes.");
            throw;
        }
    }


    private void OnBeforeSaveChanges(string userId)
    {
        this.ChangeTracker.DetectChanges();
        List<AuditEntry> auditEntryList = new List<AuditEntry>();
        foreach (EntityEntry entry in this.ChangeTracker.Entries())
        {
            if (!(entry.Entity is Audit) && entry.State != EntityState.Detached &&
                entry.State != EntityState.Unchanged)
            {
                AuditEntry auditEntry = new AuditEntry(entry)
                {
                    EntityName = entry.Entity.GetType().Name,
                    UserId = userId,
                    Metadata = "DATABASE_ACTION"
                };
                auditEntryList.Add(auditEntry);
                foreach (PropertyEntry property in entry.Properties)
                {
                    string name = property.Metadata.Name;
                    if (property.Metadata.IsPrimaryKey())
                    {
                        auditEntry.KeyValues[name] = 0;
                    }
                    else
                    {
                        switch (entry.State)
                        {
                            case EntityState.Detached:
                            case EntityState.Unchanged:
                                continue;
                            case EntityState.Deleted:
                                auditEntry.AuditType = AuditType.Delete;
                                auditEntry.OldValues[name] =
                                    property.OriginalValue ?? ""; // Default to empty string if null
                                entry.State = EntityState.Deleted;

                                continue;
                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    auditEntry.ChangedColumns.Add(name);
                                    auditEntry.AuditType = AuditType.Update;
                                    auditEntry.OldValues[name] =
                                        property.OriginalValue ?? ""; // Default to empty string if null
                                    auditEntry.NewValues[name] = property.CurrentValue ?? "";
                                    entry.State = EntityState.Modified;
                                    continue;
                                }

                                continue;
                            case EntityState.Added:
                                auditEntry.AuditType = AuditType.Create;
                                auditEntry.NewValues[name] =
                                    property.CurrentValue ?? ""; // Default to empty string if null
                                auditEntry.KeyValues[name] = 0;
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        foreach (var auditEntry in auditEntryList)
        {
            var auditEntity = auditEntry.ToAudit();
            auditEntity.Id = 0; // Reset ID to zero before adding to context.
            AuditLogs.Add(auditEntity);
        }
    }
}