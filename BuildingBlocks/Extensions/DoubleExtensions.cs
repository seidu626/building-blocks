// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.DoubleExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Common;

namespace BuildingBlocks.Extensions;

public static class DoubleExtensions
{
  public static uint GetDecimalPlaces(this double value)
  {
    Ensure.Not(double.IsNaN(value) || double.IsInfinity(value) || value.Equals(double.MaxValue) || value.Equals(double.MinValue) || value.Equals(double.Epsilon), "Invalid double value, are you sure it's not NaN, Max/Min, Epsilon or infinity? Value: " + value.ToString());
    return value.IsDefault<double>() ? 0U : (uint) BitConverter.GetBytes(Decimal.GetBits((Decimal) value)[3])[2];
  }

  public static double Floor(this double value, uint decimalPlaces)
  {
    if (decimalPlaces == 0U || double.IsNaN(value) || double.IsInfinity(value) || value.Equals(double.MaxValue) || value.Equals(double.MinValue) || value.Equals(double.Epsilon))
      return value;
    Decimal num1 = (Decimal) value;
    Decimal num2 = (Decimal) Math.Pow(10.0, (double) decimalPlaces);
    Decimal num3 = num2;
    return (double) (Math.Floor(num1 * num3) / num2);
  }

  public static double Ceiling(this double value, uint decimalPlaces)
  {
    if (decimalPlaces == 0U || double.IsNaN(value) || double.IsInfinity(value) || value.Equals(double.MaxValue) || value.Equals(double.MinValue) || value.Equals(double.Epsilon))
      return value;
    Decimal num1 = (Decimal) value;
    Decimal num2 = (Decimal) Math.Pow(10.0, (double) decimalPlaces);
    Decimal num3 = num2;
    return (double) (Math.Ceiling(num1 * num3) / num2);
  }

  public static bool EqualsFuzzy(this double left, double right, double epsilon = 1E-05)
  {
    if (left.Equals(right))
      return true;
    if (double.IsInfinity(left) || double.IsNaN(left) || double.IsInfinity(right) || double.IsNaN(right))
      return left.Equals(right);
    return Math.Abs(left - right) < epsilon;
  }
}