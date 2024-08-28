// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Contracts.Responses.ErrorResponse
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Contracts.Responses;

public class ErrorResponse
{
  public ErrorResponse()
  {
  }

  public ErrorResponse(ErrorModel error) => this.Errors.Add(error);

  public List<ErrorModel> Errors { get; set; } = new List<ErrorModel>();
}