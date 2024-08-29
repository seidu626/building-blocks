#nullable disable
namespace BuildingBlocks.Persistence.Dapper
{
  public class GuidHandler : SqliteTypeHandler<Guid>
  {
    public override Guid Parse(object value) => Guid.Parse((string) value);
  }
}
