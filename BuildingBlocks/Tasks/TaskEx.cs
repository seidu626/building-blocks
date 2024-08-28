// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Tasks.TaskEx
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Tasks;

internal static class TaskEx
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Task<TResult> Run<TResult>(
    Func<TResult> function,
    CancellationToken cancellationToken)
  {
    return Task.Run<TResult>(function, cancellationToken);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static Task Delay(TimeSpan delay, CancellationToken cancellationToken)
  {
    return Task.Delay(delay, cancellationToken);
  }

  public static Task CompletedTask
  {
    [MethodImpl(MethodImplOptions.AggressiveInlining)] get => Task.CompletedTask;
  }
}