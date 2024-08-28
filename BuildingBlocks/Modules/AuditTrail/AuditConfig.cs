#nullable disable
using BuildingBlocks.Configuration;

namespace BuildingBlocks.Modules.AuditTrail;

public class AuditConfig : IConfig
{
  public bool UseBackgroundQueue { get; set; }
}