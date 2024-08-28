// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.State.QueryExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using Microsoft.Data.SqlClient;

namespace BuildingBlocks.State;

public static class QueryExtensions
{
  public static bool HasColumn(this SqlDataReader dr, string columnName)
  {
    for (int ordinal = 0; ordinal < dr.FieldCount; ++ordinal)
    {
      if (dr.GetName(ordinal).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
        return true;
    }
    return false;
  }
}