#nullable disable
namespace BuildingBlocks.Modules.AuditTrail;

public enum AuditType
{
  None,
  Create,
  Update,
  Delete,
  Request,
}