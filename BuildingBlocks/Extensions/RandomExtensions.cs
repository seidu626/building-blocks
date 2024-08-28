// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.RandomExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Globalization;
using System.Runtime.CompilerServices;
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class RandomExtensions
{
  public static double GenerateRandomBetween(this Random random, double min, double max)
  {
    Ensure.That(min <= max, "min: " + min.ToString((IFormatProvider) CultureInfo.InvariantCulture) + " should be less than max: " + max.ToString((IFormatProvider) CultureInfo.InvariantCulture));
    return random.NextDouble() * (max - min) + min;
  }

  public static int[] GenerateRandomSequence(this Random random, int count, int min, int max)
  {
    int num1 = max <= min || count < 0 ? 1 : (count <= max - min ? 0 : (max - min > 0 ? 1 : 0));
    string[] strArray = new string[6];
    DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(25, 2);
    interpolatedStringHandler.AppendLiteral("The given range of: ");
    interpolatedStringHandler.AppendFormatted(min.ToString());
    interpolatedStringHandler.AppendLiteral(" to ");
    interpolatedStringHandler.AppendFormatted(max.ToString());
    interpolatedStringHandler.AppendLiteral(" ");
    strArray[0] = interpolatedStringHandler.ToStringAndClear();
    strArray[1] = "(";
    strArray[2] = ((long) max - (long) min).ToString();
    strArray[3] = " value(s)), with the count of: ";
    strArray[4] = count.ToString();
    strArray[5] = " is illegal.";
    string message = string.Concat(strArray);
    Ensure.Not(num1 != 0, message);
    HashSet<int> source = new HashSet<int>();
    for (int index = max - count; index < max; ++index)
    {
      if (!source.Add(random.Next(min, index + 1)))
        source.Add(index);
    }
    int[] array = source.ToArray<int>();
    for (int index1 = array.Length - 1; index1 > 0; --index1)
    {
      int index2 = random.Next(index1 + 1);
      int num2 = array[index2];
      array[index2] = array[index1];
      array[index1] = num2;
    }
    return array;
  }
}