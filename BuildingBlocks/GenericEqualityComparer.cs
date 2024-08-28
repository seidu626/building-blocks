// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.GenericEqualityComparer`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks;

public sealed class GenericEqualityComparer<T> : IEqualityComparer<T>
{
  private readonly Func<T, T, bool> _equals;
  private readonly Func<T, int> _getHashcode;

  public GenericEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashcode)
  {
    this._getHashcode = getHashcode;
    this._equals = equals;
  }

  public static GenericEqualityComparer<T> CompareMember<TMember>(
    Func<T, TMember> memberExpression)
    where TMember : IEquatable<TMember>
  {
    return new GenericEqualityComparer<T>((Func<T, T, bool>) ((x, y) => memberExpression(x).Equals(memberExpression(y))), (Func<T, int>) (x =>
    {
      TMember member = memberExpression(x);
      return (object) member == (object) default (TMember) ? 0 : member.GetHashCode();
    }));
  }

  public bool Equals(T x, T y) => this._equals(x, y);

  public int GetHashCode(T obj) => this._getHashcode(obj);
}