// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Persistence.Extensions.MiscExtensions
// Assembly: BuildingBlocks.Persistence, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 98D21131-3851-47B7-9AEB-AB489CBD4F40
// Assembly location: C:\Users\420919\Repositories\Decompiled\BuildingBlocks.Persistence.dll

using System.Linq;

#nullable disable
namespace BuildingBlocks.Persistence.Extensions
{
  public static class MiscExtensions
  {
    public static bool IsEntityFrameworkProvider(this IQueryProvider provider)
    {
      return provider.GetType().FullName == "System.Data.Objects.ELinq.ObjectQueryProvider";
    }
  }
}
