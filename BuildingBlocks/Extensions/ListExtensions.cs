// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.ListExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;

namespace BuildingBlocks.Extensions;

public static class ListExtensions
{
  [DebuggerStepThrough]
  public static void Add<T>(this IList<T> list, IEnumerable<T> items)
  {
    foreach (T obj in items)
      list.Add(obj);
  }
}