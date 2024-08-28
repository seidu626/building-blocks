// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Utilities.Numeric
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Utilities;

public static class Numeric
{
  public static bool Is(Type type)
  {
    if (type == (Type) null)
      return false;
    switch (Type.GetTypeCode(type))
    {
      case TypeCode.Object:
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Nullable<>) && Numeric.Is(Nullable.GetUnderlyingType(type));
      case TypeCode.SByte:
      case TypeCode.Byte:
      case TypeCode.Int16:
      case TypeCode.UInt16:
      case TypeCode.Int32:
      case TypeCode.UInt32:
      case TypeCode.Int64:
      case TypeCode.UInt64:
      case TypeCode.Single:
      case TypeCode.Double:
      case TypeCode.Decimal:
        return true;
      default:
        return false;
    }
  }

  public static bool Is<T>() => Numeric.Is(typeof (T));
}