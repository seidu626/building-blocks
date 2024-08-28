#nullable disable
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace BuildingBlocks.Modules.AuditTrail;

public class AuditEntry
{
  public AuditEntry(EntityEntry entry) => this.Entry = entry;

  public AuditEntry()
  {
  }

  public EntityEntry Entry { get; }

  public string UserId { get; set; }

  public string EntityName { get; set; }

  public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();

  public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();

  public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();

  public AuditType AuditType { get; set; }

  public string Metadata { get; set; }

  public List<string> ChangedColumns { get; } = new List<string>();

  public Audit ToAudit()
  {
    return new Audit()
    {
      UserId = this.UserId,
      Type = this.AuditType.ToString(),
      Metadata = this.Metadata,
      EntityName = this.EntityName,
      DateTime = DateTime.UtcNow,
      PrimaryKey = JsonConvert.SerializeObject((object) this.KeyValues),
      OldValues = this.OldValues.Count == 0 ? (string) null : JsonConvert.SerializeObject((object) this.OldValues),
      NewValues = this.NewValues.Count == 0 ? (string) null : JsonConvert.SerializeObject((object) this.NewValues),
      AffectedColumns = this.ChangedColumns.Count == 0 ? (string) null : JsonConvert.SerializeObject((object) this.ChangedColumns)
    };
  }
}