// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.Stacky
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.Serialization;
using System.Text.Json;

namespace BuildingBlocks.Exceptions;

public class Stacky
{
  private static readonly JsonSerializerOptions Options = new JsonSerializerOptions()
  {
    WriteIndented = true,
    IgnoreNullValues = true
  };

  [DataMember(Order = 0)]
  public string ExceptionType { get; set; }

  [DataMember(Order = 1)]
  public string ExceptionMessage { get; set; }

  [DataMember(Order = 2)]
  public string Method { get; set; }

  [DataMember(Order = 3)]
  public Dictionary<string, object> MethodArguments { get; set; } = new Dictionary<string, object>();

  [DataMember(Order = 4)]
  public string FileName { get; set; }

  [DataMember(Order = 5)]
  public int Line { get; set; }

  [DataMember(Order = 6)]
  public List<string> StackLines { get; set; } = new List<string>();

  public string ToJsonString() => JsonSerializer.Serialize<Stacky>(this, Stacky.Options);
}