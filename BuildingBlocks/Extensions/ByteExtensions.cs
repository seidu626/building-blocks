// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.ByteExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class ByteExtensions
{
  [DebuggerStepThrough]
  public static bool Compare(this byte[] left, byte[] right)
  {
    Ensure.NotNull<byte[]>(left, nameof (left));
    Ensure.NotNull<byte[]>(right, nameof (right));
    return left.Length == right.Length && NativeMethods.MemoryCompare(left, right, (long) left.Length) == 0;
  }
}