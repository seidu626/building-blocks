// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.NativeMethods
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.InteropServices;

namespace BuildingBlocks.Common;

internal static class NativeMethods
{
  [DllImport("rpcrt4.dll", SetLastError = true)]
  internal static extern int UuidCreateSequential(out Guid guid);

  [DllImport("Kernel32.dll")]
  internal static extern void GetSystemTimePreciseAsFileTime(out long filetime);

  [DllImport("msvcrt.dll", EntryPoint = "memcmp", CallingConvention = CallingConvention.Cdecl)]
  internal static extern int MemoryCompare(byte[] b1, byte[] b2, long count);
}