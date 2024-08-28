// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Exceptions.Errors
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Exceptions;

public static class Errors
{
    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception Application(string message, params object[] args)
    {
        return (Exception)new ApplicationException(message.FormatCurrent(args));
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception Application(
        Exception innerException,
        string message,
        params object[] args)
    {
        return (Exception)new ApplicationException(message.FormatCurrent(args), innerException);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ArgumentNullOrEmpty(Func<string> arg)
    {
        return (Exception)new ArgumentException("String parameter '{0}' cannot be null or all whitespace.",
            ((MemberInfo)arg.Method).Name);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ArgumentNull(string argName)
    {
        return (Exception)new ArgumentNullException(argName);
    }

    [DebuggerStepThrough]
    public static Exception ArgumentNull<T>(Func<T> arg)
    {
        string message = "Argument of type '{0}' cannot be null".FormatInvariant((object)typeof(T));
        return (Exception)new ArgumentNullException(((MemberInfo)arg.Method).Name, message);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ArgumentOutOfRange<T>(Func<T> arg)
    {
        return (Exception)new ArgumentOutOfRangeException(((MemberInfo)arg.Method).Name);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ArgumentOutOfRange(string argName)
    {
        return (Exception)new ArgumentOutOfRangeException(argName);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ArgumentOutOfRange(
        string argName,
        string message,
        params object[] args)
    {
        return (Exception)new ArgumentOutOfRangeException(argName,
            string.Format((IFormatProvider)CultureInfo.CurrentCulture, message, args));
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception Argument(string argName, string message, params object[] args)
    {
        return (Exception)new ArgumentException(
            string.Format((IFormatProvider)CultureInfo.CurrentCulture, message, args), argName);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception Argument<T>(Func<T> arg, string message, params object[] args)
    {
        return (Exception)new ArgumentException(message.FormatCurrent(args), ((MemberInfo)arg.Method).Name);
    }

    [DebuggerStepThrough]
    public static Exception InvalidOperation(string message, params object[] args)
    {
        return Errors.InvalidOperation(message, (Exception)null, args);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception InvalidOperation(
        string message,
        Exception innerException,
        params object[] args)
    {
        return (Exception)new InvalidOperationException(message.FormatCurrent(args), innerException);
    }

    [DebuggerStepThrough]
    public static Exception InvalidOperation<T>(string message, Func<T> member)
    {
        return Errors.InvalidOperation<T>(message, (Exception)null, member);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception InvalidOperation<T>(
        string message,
        Exception innerException,
        Func<T> member)
    {
        Guard.AgainstNull((object)message, nameof(message));
        Guard.AgainstNull((object)member, nameof(member));
        return (Exception)new InvalidOperationException(message.FormatCurrent((object)((MemberInfo)member.Method).Name),
            innerException);
    }

    [DebuggerStepThrough]
    public static Exception InvalidCast(Type fromType, Type toType)
    {
        return Errors.InvalidCast(fromType, toType, (Exception)null);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception InvalidCast(Type fromType, Type toType, Exception innerException)
    {
        return (Exception)new InvalidCastException(
            "Cannot convert from type '{0}' to '{1}'.".FormatCurrent((object)(fromType?.FullName ?? "NULL"),
                (object)toType.FullName), innerException);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception NotSupported() => (Exception)new NotSupportedException();

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception NotImplemented() => (Exception)new NotImplementedException();

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ObjectDisposed(string objectName)
    {
        return (Exception)new ObjectDisposedException(objectName);
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception ObjectDisposed(string objectName, string message, params object[] args)
    {
        return (Exception)new ObjectDisposedException(objectName,
            string.Format((IFormatProvider)CultureInfo.CurrentCulture, message, args));
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception NoElements()
    {
        return (Exception)new InvalidOperationException("Sequence contains no elements.");
    }

    [DebuggerStepThrough]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Exception MoreThanOneElement()
    {
        return (Exception)new InvalidOperationException("Sequence contains more than one element.");
    }
}