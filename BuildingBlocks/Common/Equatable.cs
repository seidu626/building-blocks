// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.Equatable`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Common;

public abstract class Equatable<T> : IEquatable<T>
{
  public abstract override int GetHashCode();

  public virtual bool Equals(T other)
  {
    return (object) other != null && other.GetHashCode() == this.GetHashCode();
  }

  public override bool Equals(object obj) => obj is T other && this.Equals(other);

  public static bool operator ==(Equatable<T> left, Equatable<T> right)
  {
    return object.Equals((object) left, (object) right);
  }

  public static bool operator !=(Equatable<T> left, Equatable<T> right)
  {
    return !object.Equals((object) left, (object) right);
  }
}