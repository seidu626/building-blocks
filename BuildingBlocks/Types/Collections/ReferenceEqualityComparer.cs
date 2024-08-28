using System.Collections;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Types.Collections;

public sealed class ReferenceEqualityComparer : IEqualityComparer, IEqualityComparer<object>
{
    public static ReferenceEqualityComparer Default { get; } = new ReferenceEqualityComparer();

    public new bool Equals(object x, object y)
    {
        return ReferenceEquals(x, y);
    }

    public int GetHashCode(object obj)
    {
        return RuntimeHelpers.GetHashCode(obj);
    }
}