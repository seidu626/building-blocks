#nullable enable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BuildingBlocks.Modules.AuditTrail;

public abstract class AuditableIdentityContext : DbContext
{
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

    protected virtual async Task<int> SaveChangesAsync(
        string userId,
        CancellationToken cancellationToken = default(CancellationToken))
    {
        this.OnBeforeSaveChanges(userId);
        // ISSUE: reference to a compiler-generated method
        return await this.SaveChangesAsync(cancellationToken);
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
                        auditEntry.KeyValues[name] = property.CurrentValue;
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
                                auditEntry.OldValues[name] = property.OriginalValue;
                                continue;
                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    auditEntry.ChangedColumns.Add(name);
                                    auditEntry.AuditType = AuditType.Update;
                                    auditEntry.OldValues[name] = property.OriginalValue;
                                    auditEntry.NewValues[name] = property.CurrentValue;
                                    continue;
                                }

                                continue;
                            case EntityState.Added:
                                auditEntry.AuditType = AuditType.Create;
                                auditEntry.NewValues[name] = property.CurrentValue;
                                continue;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }

        foreach (AuditEntry auditEntry in auditEntryList)
            this.AuditLogs.Add(auditEntry.ToAudit());
    }
}