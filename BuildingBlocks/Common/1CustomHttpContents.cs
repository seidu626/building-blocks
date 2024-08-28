// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.XmlContent
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Text;

namespace BuildingBlocks.Common;

public sealed class XmlContent : StringContent
{
  private const string XmlMime = "application/xml";

  public XmlContent(string xmlContent)
    : base(xmlContent, Encoding.UTF8, "application/xml")
  {
  }

  public XmlContent(string xmlContent, Encoding encoding)
    : base(xmlContent, encoding, "application/xml")
  {
  }
}