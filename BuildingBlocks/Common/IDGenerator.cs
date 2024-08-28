// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.IdGenerator
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public sealed class IdGenerator
{
  private const string Encode32Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUV";
  private static readonly char[] Prefix = new char[6];
  private static long _lastId = DateTime.UtcNow.Ticks;
  private static readonly ThreadLocal<char[]> CharBufferThreadLocal = new ThreadLocal<char[]>((Func<char[]>) (() =>
  {
    char[] chArray = new char[20]
    {
      IdGenerator.Prefix[0],
      IdGenerator.Prefix[1],
      IdGenerator.Prefix[2],
      IdGenerator.Prefix[3],
      IdGenerator.Prefix[4],
      IdGenerator.Prefix[5],
      '-',
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue,
      char.MinValue
    };
    return chArray;
  }));

  static IdGenerator() => IdGenerator.PopulatePrefix();

  private IdGenerator()
  {
  }

  public static IdGenerator Instance { get; } = new IdGenerator();

  public string Next => IdGenerator.GenerateImpl(Interlocked.Increment(ref IdGenerator._lastId));

  private static string GenerateImpl(long id)
  {
    char[] chArray = IdGenerator.CharBufferThreadLocal.Value;
    chArray[7] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 60) & 31];
    chArray[8] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 55) & 31];
    chArray[9] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 50) & 31];
    chArray[10] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 45) & 31];
    chArray[11] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 40) & 31];
    chArray[12] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 35) & 31];
    chArray[13] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 30) & 31];
    chArray[14] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 25) & 31];
    chArray[15] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 20) & 31];
    chArray[16] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 15) & 31];
    chArray[17] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 10) & 31];
    chArray[18] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) (id >> 5) & 31];
    chArray[19] = "0123456789ABCDEFGHIJKLMNOPQRSTUV"[(int) id & 31];
    return new string(chArray, 0, chArray.Length);
  }

  private static void PopulatePrefix()
  {
    string str = Base36.Encode((long) Math.Abs(Environment.MachineName.GetHashCode()));
    int index1 = IdGenerator.Prefix.Length - 1;
    int index2 = 0;
    for (; index1 >= 0; --index1)
    {
      if (index2 < str.Length)
      {
        IdGenerator.Prefix[index1] = str[index2];
        ++index2;
      }
      else
        IdGenerator.Prefix[index1] = '0';
    }
  }
}