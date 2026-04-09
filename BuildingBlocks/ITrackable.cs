#nullable disable
namespace BuildingBlocks;

public interface ITrackable
{
  DateTime CreatedAt { get; }
  DateTime LastUpdatedAt { get; }
  string CreatedBy { get; }
  string LastUpdatedBy { get; }
}