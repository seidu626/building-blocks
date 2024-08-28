// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.GenericTypeExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Extensions;

public static class GenericTypeExtensions
{
  public static string GetGenericTypeName(this Type type)
  {
    string empty = string.Empty;
    string genericTypeName;
    if (type.IsGenericType)
    {
      string str = string.Join(",", ((IEnumerable<Type>) type.GetGenericArguments()).Select<Type, string>((Func<Type, string>) (t => t.Name)).ToArray<string>());
      genericTypeName = type.Name.Remove(type.Name.IndexOf('`')) + "<" + str + ">";
    }
    else
      genericTypeName = type.Name;
    return genericTypeName;
  }

  public static string GetGenericTypeName(this object @object)
  {
    return GenericTypeExtensions.GetGenericTypeName(@object.GetType());
  }
}