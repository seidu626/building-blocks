// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.Guard
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Exceptions;

public static class Guard
{
  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstInRange<T>(T arg, T min, T max, string argName) where T : struct, IComparable<T>
  {
    if (arg.CompareTo(min) < 0 || arg.CompareTo(max) > 0)
      throw Errors.ArgumentOutOfRange(argName, "The argument '{0}' must be between '{1}' and '{2}'.", (object) argName, (object) min, (object) max);
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstOutOfLength(string arg, int maxLength, string argName)
  {
    if (arg.Trim().Length > maxLength)
      throw Errors.Argument(argName, "Argument '{0}' cannot be more than {1} characters long.", (object) argName, (object) maxLength);
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstNegative<T>(T arg, string argName, string message = "Argument '{0}' cannot be a negative value. Value: '{1}'.") where T : struct, IComparable<T>
  {
    if (arg.CompareTo(default (T)) < 0)
      throw Errors.ArgumentOutOfRange(argName, message.FormatInvariant((object) argName, (object) arg));
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstPositive<T>(T arg, string argName, string message = "Argument '{0}' must be a positive value. Value: '{1}'.") where T : struct, IComparable<T>
  {
    if (arg.CompareTo(default (T)) >= 0)
      throw Errors.ArgumentOutOfRange(argName, message.FormatInvariant((object) argName));
  }

  [DebuggerStepThrough]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstZero<T>(T arg, string argName) where T : struct, IComparable<T>
  {
    if (arg.CompareTo(default (T)) == 0)
      throw Errors.ArgumentOutOfRange(argName, "Argument '{0}' must be greater or less than zero. Value: '{1}'.", (object) argName, (object) arg);
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstNull(object argumentValue, string argumentName)
  {
    if (argumentValue == null)
      throw new ArgumentNullException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstEmpty<T>(ArraySegment<T> argumentValue, string argumentName)
  {
    if (argumentValue.Count == 0)
      throw new ArgumentException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstEmpty<T>(ReadOnlyMemory<T> argumentValue, string argumentName)
  {
    if (argumentValue.Length == 0)
      throw new ArgumentException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstNullOrEmpty<T>(IEnumerable<T> argumentValue, string argumentName)
  {
    if (argumentValue != null && !argumentValue.Any<T>())
      throw new ArgumentException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstNullOrEmpty(string argumentValue, string argumentName)
  {
    if (string.IsNullOrEmpty(argumentValue))
      throw new ArgumentException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstNullOrEmpty(Stream argumentValue, string argumentName)
  {
    if (argumentValue == null || argumentValue.Length == 0L)
      throw new ArgumentException(Strings.Write("{0} can't be null or empty.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstBothNullOrEmpty(
    string argumentValue,
    string argumentName,
    string secondArgumentValue,
    string secondArgumentName)
  {
    if (string.IsNullOrEmpty(argumentValue) && string.IsNullOrEmpty(secondArgumentValue))
      throw new ArgumentException(Strings.Write("Both {0} and {1} can't be null or empty.", argumentName, secondArgumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstTrue(bool argumentValue, string argumentName)
  {
    if (argumentValue)
      throw new ArgumentException(Strings.Write("{0} can't be true for this method.", argumentName));
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static void AgainstFalse(bool argumentValue, string argumentName)
  {
    if (argumentValue)
      throw new ArgumentException(Strings.Write("{0} can't be true for this method.", argumentName));
  }
}