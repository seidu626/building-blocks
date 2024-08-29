#nullable disable
namespace BuildingBlocks.Persistence.Dapper
{
  public class TimeSpanHandler : SqliteTypeHandler<TimeSpan>
  {
    public override TimeSpan Parse(object value) => TimeSpan.Parse((string) value);
  }
}
