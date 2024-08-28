// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Base36
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public static class Base36
{
  private const string Base36Characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

  public static string Encode(long input)
  {
    Ensure.That<ArgumentException>(input >= 0L, "Input cannot be negative.");
    char[] charArray = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    Stack<char> charStack = new Stack<char>();
    for (; input != 0L; input /= 36L)
      charStack.Push(charArray[input % 36L]);
    return new string(charStack.ToArray());
  }

  public static long Decode(string input)
  {
    Ensure.NotNull<string>(input, nameof (input));
    IEnumerable<char> chars = input.ToUpper().Reverse<char>();
    long num1 = 0;
    int num2 = 0;
    foreach (char ch in chars)
    {
      num1 += (long) "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".IndexOf(ch) * (long) Math.Pow(36.0, (double) num2);
      ++num2;
    }
    return num1;
  }
}