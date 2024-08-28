// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Helpers.ThreadLocalRandom
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Helpers;

public static class ThreadLocalRandom
{
  private static readonly Random GlobalRandom = new Random();
  private static readonly object GlobalLock = new object();
  private static readonly ThreadLocal<Random> ThreadRandom = new ThreadLocal<Random>(new Func<Random>(ThreadLocalRandom.NewRandom));

  public static Random NewRandom()
  {
    lock (ThreadLocalRandom.GlobalLock)
      return new Random(ThreadLocalRandom.GlobalRandom.Next());
  }

  public static Random Instance => ThreadLocalRandom.ThreadRandom.Value;

  public static int Next() => ThreadLocalRandom.Instance.Next();

  public static int Next(int maxValue) => ThreadLocalRandom.Instance.Next(maxValue);

  public static int Next(int minValue, int maxValue)
  {
    return ThreadLocalRandom.Instance.Next(minValue, maxValue);
  }

  public static double NextDouble() => ThreadLocalRandom.Instance.NextDouble();

  public static void NextBytes(byte[] buffer) => ThreadLocalRandom.Instance.NextBytes(buffer);
}