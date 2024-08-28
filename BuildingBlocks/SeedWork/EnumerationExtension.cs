// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.SeedWork.EnumerationExtension
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using BuildingBlocks.Exceptions;

namespace BuildingBlocks.SeedWork;

public static class EnumerationExtension
{
    public static Enumeration<TType, TValue> FromDisplayName<TType, TValue>(
        this Enumeration<TType, TValue> @this,
        string name)
        where TType : Enumeration<TType, TValue>
        where TValue : IComparable
    {
        if (@this == (Enumeration<TType, TValue>)null)
            throw new ArgumentNullException(nameof(@this));
        TType[] all = Enumeration<TType, TValue>.GetAll();
        TType type = ((IEnumerable<TType>)all).FirstOrDefault<TType>((Func<TType, bool>)(s =>
            string.Equals(s.DisplayName, name, StringComparison.CurrentCultureIgnoreCase)));
        return !((Enumeration<TType, TValue>)type == (Enumeration<TType, TValue>)null)
            ? (Enumeration<TType, TValue>)type
            : throw new DomainException("Possible values for this: " + string.Join(",",
                ((IEnumerable<TType>)all).Select<TType, string>((Func<TType, string>)(s => s.DisplayName))));
    }

    public static Enumeration<TType, TValue> FromValue<TType, TValue>(
        this Enumeration<TType, TValue> @this,
        TValue value)
        where TType : Enumeration<TType, TValue>
        where TValue : IComparable
    {
        if (@this == (Enumeration<TType, TValue>)null)
            throw new ArgumentNullException(nameof(@this));
        TType[] all = Enumeration<TType, TValue>.GetAll();
        TType type =
            ((IEnumerable<TType>)all).FirstOrDefault<TType>((Func<TType, bool>)(s =>
                s.Value.Equals((object)(TValue)value)));
        return !((Enumeration<TType, TValue>)type == (Enumeration<TType, TValue>)null)
            ? (Enumeration<TType, TValue>)type
            : throw new DomainException("Possible values for this: " + string.Join(",",
                ((IEnumerable<TType>)all).Select<TType, string>((Func<TType, string>)(s => s.DisplayName))));
    }
}