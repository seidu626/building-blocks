using System.Diagnostics;
using System.Reflection;
using System.Runtime.Serialization;

namespace BuildingBlocks.SeedWork
{
    [DebuggerDisplay("{DisplayName} - {Value}")]
    [Serializable]
    public abstract class Enumeration<TType> : Enumeration<TType, int>
        where TType : Enumeration<TType>
    {
        protected Enumeration(int value, string displayName) : base(value, displayName) { }

        public static int AbsoluteDifference(Enumeration<TType, int> firstValue, Enumeration<TType, int> secondValue)
        {
            return Math.Abs(firstValue.Value - secondValue.Value);
        }
    }

    [DebuggerDisplay("{DisplayName} - {Value}")]
    [DataContract(Namespace = "http://github.com/HeadspringLabs/Enumeration/5/13")]
    [Serializable]
    public abstract class Enumeration<TType, TValue> : IComparable<TType>, IEquatable<TType>
        where TType : Enumeration<TType, TValue>
        where TValue : IComparable
    {
        private static readonly Lazy<TType[]> Enumerations = new Lazy<TType[]>(GetEnumerations);

        protected Enumeration(TValue value, string displayName)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
        }

        [DataMember(Order = 0)]
        public TValue Value { get; }

        [DataMember(Order = 1)]
        public string DisplayName { get; }

        public int CompareTo(TType other) => Value.CompareTo(other.Value);

        public override string ToString() => DisplayName;

        public static TType[] GetAll() => Enumerations.Value;

        private static TType[] GetEnumerations()
        {
            return typeof(TType).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
                .Where(info => info.FieldType == typeof(TType))
                .Select(info => (TType)info.GetValue(null))
                .ToArray();
        }

        public override bool Equals(object obj) => Equals(obj as TType);

        public bool Equals(TType other) => other != null && ValueEquals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(Enumeration<TType, TValue> left, Enumeration<TType, TValue> right) => Equals(left, right);

        public static bool operator !=(Enumeration<TType, TValue> left, Enumeration<TType, TValue> right) => !Equals(left, right);

        public static TType FromDisplayName(string displayName) => Parse(displayName, "display name", item => item.DisplayName == displayName);

        public static TType FromValue(TValue value) => Parse(value, "value", item => item.Value.Equals(value));

        private static TType Parse(object value, string description, Func<TType, bool> predicate)
        {
            if (!TryParse(predicate, out TType result))
            {
                throw new ArgumentException($"'{value}' is not a valid {description} in {typeof(TType).Name}", nameof(value));
            }
            return result;
        }

        public static bool TryParse(Func<TType, bool> predicate, out TType result)
        {
            result = GetAll().FirstOrDefault(predicate);
            return result != null;
        }

        protected virtual bool ValueEquals(TValue otherValue) => Value.Equals(otherValue);
    }
}
