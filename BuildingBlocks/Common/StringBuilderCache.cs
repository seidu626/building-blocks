// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.StringBuilderCache
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Text;

namespace BuildingBlocks.Common;

public static class StringBuilderCache
{
  [ThreadStatic]
  private static StringBuilder _cache;

  [DebuggerStepThrough]
  public static StringBuilder Acquire()
  {
    StringBuilder cache = StringBuilderCache._cache;
    if (cache == null)
      return new StringBuilder();
    cache.Clear();
    StringBuilderCache._cache = (StringBuilder) null;
    return cache;
  }

  [DebuggerStepThrough]
  public static string GetStringAndRelease(StringBuilder builder)
  {
    string stringAndRelease = builder.ToString();
    StringBuilderCache._cache = builder;
    return stringAndRelease;
  }
}