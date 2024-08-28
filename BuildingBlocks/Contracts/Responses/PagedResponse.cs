// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Contracts.Responses.PagedResponse`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Contracts.Responses;

public class PagedResponse<T>
{
  public PagedResponse()
  {
  }

  public PagedResponse(IEnumerable<T> data) => this.Data = data;

  public IEnumerable<T> Data { get; set; }

  public int? PageNumber { get; set; }

  public int? PageSize { get; set; }

  public string NextPage { get; set; }

  public string PreviousPage { get; set; }
}