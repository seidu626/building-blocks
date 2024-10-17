// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.SeedWork.ValueObject
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Exceptions;

public abstract class ValueObject
{
    protected static bool EqualOperator(ValueObject left, ValueObject right)
    {
        if ((object)left == null ^ (object)right == null)
            return false;
        return (object)left == null || left.Equals((object)right);
    }

    protected static bool NotEqualOperator(ValueObject left, ValueObject right)
    {
        return !ValueObject.EqualOperator(left, right);
    }

    protected abstract IEnumerable<object> GetEqualityComponents();

    public override bool Equals(object obj)
    {
        if (obj == null || obj.GetType() != this.GetType())
            return false;
        ValueObject valueObject = (ValueObject)obj;
        return this.GetEqualityComponents().SequenceEqual<object>(valueObject.GetEqualityComponents());
    }

    public override int GetHashCode()
    {
        return this.GetEqualityComponents()
            .Select<object, int>((Func<object, int>)(x => x == null ? 0 : x.GetHashCode()))
            .Aggregate<int>((Func<int, int, int>)((x, y) => x ^ y));
    }

    public ValueObject GetCopy() => this.MemberwiseClone() as ValueObject;

    public static bool operator ==(ValueObject a, ValueObject b) => ValueObject.EqualOperator(a, b);

    public static bool operator !=(ValueObject a, ValueObject b)
    {
        return ValueObject.NotEqualOperator(a, b);
    }
}