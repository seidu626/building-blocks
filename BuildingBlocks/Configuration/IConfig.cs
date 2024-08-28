using Newtonsoft.Json;

namespace BuildingBlocks.Configuration;

public interface IConfig
{
  [JsonIgnore]
  string Name => this.GetType().Name;
}