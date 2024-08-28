// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.XmlExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
using System.Xml;
using System.Xml.Linq;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class XmlExtensions
{
  public static void SetDefaultXmlNamespace(this 
#nullable disable
    XElement element, XNamespace xmlns)
  {
    Ensure.NotNull<XElement>(element, nameof (element));
    Ensure.NotNull<XNamespace>(xmlns, nameof (xmlns));
    if (element.Name.NamespaceName == string.Empty)
      element.Name = xmlns + element.Name.LocalName;
    foreach (XElement element1 in element.Elements())
      element1.SetDefaultXmlNamespace(xmlns);
  }

  public static IEnumerable<XElement> GetEelements(
    this XmlReader reader,
    XName name,
    bool ignoreCase = true)
  {
    Ensure.NotNull<XmlReader>(reader, nameof (reader));
    Ensure.NotNull<XName>(name, nameof (name));
    StringComparison compPolicy = ignoreCase ? StringComparison.InvariantCultureIgnoreCase : StringComparison.InvariantCulture;
    reader.MoveToElement();
    label_4:
    while (reader.Read())
    {
      while (true)
      {
        if (reader.NodeType == XmlNodeType.Element && reader.Name.Equals(name.LocalName, compPolicy))
          yield return (XElement) XNode.ReadFrom(reader);
        else
          goto label_4;
      }
    }
  }

  public static DynamicDictionary ToDynamic(this XmlReader reader, bool ignoreCase = true)
  {
    Ensure.NotNull<XmlReader>(reader, nameof (reader));
    DynamicDictionary dynamic = new DynamicDictionary(ignoreCase);
    List<XElement> xelementList = new List<XElement>();
    dynamic["Elements"] = (object) xelementList;
    reader.MoveToElement();
    while (reader.Read())
    {
      while (reader.NodeType == XmlNodeType.Element)
      {
        XElement xelement = (XElement) XNode.ReadFrom(reader);
        xelementList.Add(xelement);
      }
    }
    return dynamic;
  }
}