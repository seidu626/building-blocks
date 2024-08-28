
#nullable disable
using System.Collections.Concurrent;

namespace BuildingBlocks.Modules.AuditTrail;

public class BackgroundAuditQueue
{
  private readonly ConcurrentQueue<AuditEntry> _auditEntries = new ConcurrentQueue<AuditEntry>();

  public void QueueAuditEntry(AuditEntry entry) => this._auditEntries.Enqueue(entry);

  public bool TryDequeue(out AuditEntry entry) => this._auditEntries.TryDequeue(out entry);

  public int Size => this._auditEntries.Count;
}