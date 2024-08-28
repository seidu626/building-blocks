// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Contracts.Responses.ErrorModel
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Newtonsoft.Json;

namespace BuildingBlocks.Contracts.Responses;

public class ErrorModel
{
  public string FieldName { get; set; }

  public string Message { get; set; }

  public int StatusCode { get; set; }

  public override string ToString() => JsonConvert.SerializeObject((object) this);
}