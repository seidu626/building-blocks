// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.JsonContent
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text;

namespace BuildingBlocks.Common;

public sealed class JsonContent : StringContent
{
  private const string JsonMime = "application/json";

  public JsonContent(string jsonContent)
    : base(jsonContent, Encoding.UTF8, "application/json")
  {
  }

  public JsonContent(string jsonContent, Encoding encoding)
    : base(jsonContent, encoding, "application/json")
  {
  }
}