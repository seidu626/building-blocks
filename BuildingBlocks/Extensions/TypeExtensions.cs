using System.Collections;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using BuildingBlocks.Common;
using BuildingBlocks.Exceptions;
using BuildingBlocks.Infrastructure;

namespace BuildingBlocks.Extensions;

/// <summary>
/// Extension methods for <see cref="Type"/>.
/// </summary>
public static class TypeExtensions
{
    private static readonly Type[] s_predefinedTypes = new Type[] { typeof(string), typeof(decimal), typeof(DateTime), typeof(TimeSpan), typeof(Guid) };
    private static readonly Type[] SimpleTypes =
    {
        typeof(byte),
        typeof(sbyte),
        typeof(short),
        typeof(ushort),
        typeof(int),
        typeof(uint),
        typeof(long),
        typeof(ulong),
        typeof(float),
        typeof(double),
        typeof(decimal),
        typeof(bool),
        typeof(string),
        typeof(char),
        typeof(Guid),
        typeof(DateTime),
        typeof(DateTimeOffset),
        typeof(TimeSpan),
        typeof(byte[])
    };

    private static readonly Dictionary<string, SequenceType> NonGenericCollectionsToSequenceTypeMapping = new Dictionary<string, SequenceType>(StringComparer.Ordinal)
    {
        { "System.String", SequenceType.String },
        { "System.Collections.ArrayList", SequenceType.ArrayList },
        { "System.Collections.Queue", SequenceType.Queue },
        { "System.Collections.Stack", SequenceType.Stack },
        { "System.Collections.BitArray", SequenceType.BitArray },
        { "System.Collections.SortedList", SequenceType.SortedList },
        { "System.Collections.Hashtable", SequenceType.Hashtable },
        { "System.Collections.Specialized.ListDictionary", SequenceType.ListDictionary },
        { "System.Collections.IList", SequenceType.IList },
        { "System.Collections.ICollection", SequenceType.ICollection },
        { "System.Collections.IDictionary", SequenceType.IDictionary },
        { "System.Collections.IEnumerable", SequenceType.IEnumerable }
    };

    /// <summary>
    /// Returns the <c>instance</c> property of the given <paramref name="type"/> regardless of it's access modifier.
    /// <remarks>This method can be used to return both a <c>public</c> or <c>non-public</c> property.</remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static bool TryGetInstanceProperty(this Type type, string? propertyName, out PropertyInfo property, bool inherit = true)
    {
        Ensure.NotNull(type, nameof(type));
        Ensure.NotNullOrEmptyOrWhiteSpace(propertyName);

        var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        if (!inherit) { flags = flags | BindingFlags.DeclaredOnly; }

        property = type.GetProperties(flags)
            .FirstOrDefault(p => p.Name.Equals(propertyName, StringComparison.Ordinal));

        return property != null;
    }

    /// <summary>
    /// Returns all <c>instance</c> properties of the given <paramref name="type"/>.
    /// <remarks>This method can be used to return both a <c>public</c> or <c>non-public</c> property.</remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static PropertyInfo[] GetInstanceProperties(this Type type, bool inherit = true, bool includePrivate = true)
    {
        Ensure.NotNull(type, nameof(type));
        return GetInstanceProperties(type.GetTypeInfo(), inherit, includePrivate);
    }

    /// <summary>
    /// Returns all <c>instance</c> properties of the given <paramref name="typeInfo"/>.
    /// <remarks>This method can be used to return both a <c>public</c> or <c>non-public</c> property.</remarks>
    /// </summary>
    [DebuggerStepThrough]
    public static PropertyInfo[] GetInstanceProperties(this TypeInfo typeInfo, bool inherit, bool includePrivate)
    {
        Ensure.NotNull(typeInfo, nameof(typeInfo));

        var flags = BindingFlags.Instance | BindingFlags.Public;
        if (includePrivate) { flags = flags | BindingFlags.NonPublic; }
        if (!inherit) { flags = flags | BindingFlags.DeclaredOnly; }

        return typeInfo.GetProperties(flags);
    }

    /// <summary>
    /// Returns the properties marked with an attribute of type <typeparamref name="T"/>.
    /// <remarks>It avoids materializing any attribute instances. <see href="http://stackoverflow.com/a/2282254/1226568"/></remarks>
    /// </summary>
    /// <typeparam name="T">Type of <c>Attribute</c> which has decorated the properties.</typeparam>
    /// <param name="type">Type of <c>Object</c> which has properties decorated with <typeparamref name="T"/>.</param>
    /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
    /// <returns>A sequence containing properties decorated with <typeparamref name="T"/>.</returns>
    [DebuggerStepThrough]
    public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(this Type type, bool inherit = true) where T : Attribute
    {
        Ensure.NotNull(type, nameof(type));
        return type.GetProperties()
            .Where(prop => Attribute.IsDefined(prop, typeof(T)) && (prop.DeclaringType == type || inherit));
    }

    /// <summary>
    /// Returns a mapping of <typeparamref name="T"/> attribute to <see cref="PropertyInfo"/> for a given <paramref name="type"/>.
    /// </summary>
    /// <typeparam name="T">Type of attribute which will be used as the key</typeparam>
    /// <param name="type">Type whose properties will be mapped to the <typeparamref name="T"/> attributes</param>
    /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
    /// <returns>A mapping between the attributes defined on the properties and the property infos</returns>
    [DebuggerStepThrough]
    public static Dictionary<T, PropertyInfo> GetAttributeToPropertyMapping<T>(this Type type, bool inherit = true) where T : Attribute
    {
        Ensure.NotNull(type, nameof(type));

        var properties = type.GetProperties();
        var result = new Dictionary<T, PropertyInfo>(properties.Length);
        foreach (var property in properties)
        {
            var attributes = property.GetCustomAttributes<T>(inherit);
            var attr = attributes.FirstOrDefault();
            if (attr is null) { continue; }

            result[attr] = property;
        }
        return result;
    }

    /// <summary>
    /// Tries to get attributes of type <typeparamref name="T"/> defined on the given <paramref name="type"/>.
    /// </summary>
    /// <typeparam name="T">The type of the attribute to get</typeparam>
    /// <param name="type">The type on which the attribute has been defined</param>
    /// <param name="attributes">All of the attributes found on the given type</param>
    /// <param name="inherit">If <c>true</c> it also searches the ancestors for the <typeparamref name="T"/>.</param>
    /// <returns><c>True</c> if successful otherwise <c>False</c></returns>
    [DebuggerStepThrough]
    public static bool TryGetAttributes<T>(this Type type, out T[] attributes, bool inherit = true) where T : Attribute
    {
        var result = Attribute.GetCustomAttributes(type, typeof(T), inherit);

        if (result.Length > 0)
        {
            attributes = result as T[];
            return true;
        }

        attributes = null;
        return false;
    }

    /// <summary>
    /// Tries to get the generic type arguments for the given <paramref name="type"/>.
    /// <example>For a type of <see cref="List{Int32}"/> the generic type is <see cref="int"/>.</example>
    /// </summary>
    /// <param name="type">The type for which generic type should be retrieved</param>
    /// <param name="genericArguments">The result</param>
    /// <returns><see langword="true"/> if generic types can be retrieved otherwise <see langword="false"/></returns>
    [DebuggerStepThrough]
    public static bool TryGetGenericArguments(this Type type, out Type[] genericArguments)
    {
        Ensure.NotNull(type, nameof(type));

        if (type.IsArray)
        {
            genericArguments = new[] { type.GetElementType() };
            return true;
        }

        if (!type.IsGenericType)
        {
            genericArguments = null;
            return false;
        }

        genericArguments = type.GetGenericArguments();
        return true;
    }

    /// <summary>
    /// Determines if the given <paramref name="type"/> is a sequence of elements.
    /// </summary>
    /// <param name="type">The type to inspect</param>
    /// <param name="sequenceType">The determined type of the sequence</param>
    /// <returns><c>True</c> if <paramref name="type"/> is a sequence otherwise <c>False</c></returns>
    [DebuggerStepThrough]
    public static bool IsSequence(this Type type, out SequenceType sequenceType)
    {
        Ensure.NotNull(type, nameof(type));

        if (type.IsArray)
        {
            sequenceType = SequenceType.Array;
            return true;
        }

        if (NonGenericCollectionsToSequenceTypeMapping.TryGetValue(type.FullName, out sequenceType))
        {
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.List`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericList;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.HashSet`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericHashSet;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.ObjectModel.Collection`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericCollection;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.LinkedList`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericLinkedList;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.Stack`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericStack;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.Queue`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericQueue;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.IList`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericIList;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericICollectionKeyValue;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.ICollection`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericICollection;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1[[System.Collections.Generic.KeyValuePair`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericIEnumerableKeyValue;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.IEnumerable`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericIEnumerable;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.Dictionary`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericDictionary;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.SortedDictionary`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericSortedDictionary;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.SortedList`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericSortedList;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.IDictionary`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericIDictionary;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Generic.ICollection`2", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericIDictionary;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Concurrent.BlockingCollection`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericBlockingCollection;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentBag`1", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericConcurrentBag;
            return true;
        }

        if (type.FullName.StartsWith("System.Collections.Concurrent.ConcurrentDictionary`2[[", StringComparison.Ordinal))
        {
            sequenceType = SequenceType.GenericConcurrentDictionary;
            return true;
        }

        var interfaces = type.GetInterfaces().ToArray();

        if (interfaces.Any(i => i.Name.StartsWith("IEnumerable`1", StringComparison.Ordinal)))
        {
            sequenceType = SequenceType.GenericCustom;
            return true;
        }

        if (interfaces.Any(i => i.Name.StartsWith("IEnumerable", StringComparison.Ordinal)))
        {
            sequenceType = SequenceType.Custom;
            return true;
        }

        sequenceType = SequenceType.Invalid;
        return false;
    }

    /// <summary>
    /// Determines whether the <paramref name="type"/> implements <typeparamref name="T"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static bool Implements<T>(this Type type)
    {
        Ensure.NotNull(type, nameof(type));
        return typeof(T).IsAssignableFrom(type);
    }
        

    /// <summary>
    /// Determines whether the given <paramref name="type"/> is of simple type.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns><c>True</c> if it is simple type otherwise <c>False</c>.</returns>
    [DebuggerStepThrough]
    public static bool IsSimpleType(this Type type)
    {
        Ensure.NotNull(type, nameof(type));
        var underlyingType = Nullable.GetUnderlyingType(type);
        type = underlyingType ?? type;

        return Array.IndexOf(SimpleTypes, type) > -1 || type.IsEnum;
    }

    /// <summary>
    /// Determines whether the given <paramref name="type"/> an array of <typeparamref name="T"/>
    /// </summary>
    /// <typeparam name="T">The Type of the elements in the array.</typeparam>
    /// <param name="type">The type of object.</param>
    /// <returns><c>True</c> or <c>False</c></returns>
    [DebuggerStepThrough]
    public static bool IsArrayOf<T>(this Type type) => type == typeof(T[]);

    /// <summary>
    /// Determines whether the given <paramref name="type"/> is a generic list
    /// </summary>
    /// <param name="type">The type to evaluate</param>
    /// <returns><c>True</c> if is generic otherwise <c>False</c></returns>
    [DebuggerStepThrough]
    public static bool IsGenericList(this Type type)
    {
        if (!type.IsGenericType) { return false; }

        var typeDef = type.GetGenericTypeDefinition();

        if (typeDef == typeof(List<>) || typeDef == typeof(IList<>)) { return true; }

        return false;
    }
        
    /// <summary>
    /// Determines if the given <paramref name="type"/> is numeric.
    /// </summary>
    [DebuggerStepThrough]
    public static bool IsNumeric(this Type type)
    {
        if (type is null) { return false; }

        var underlyingType = Nullable.GetUnderlyingType(type) ?? type;
        if (underlyingType.GetTypeInfo().IsEnum) { return false; }

        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (underlyingType.GetTypeCode())
        {
            case TypeCode.Byte:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.SByte:
            case TypeCode.Single:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Gets the <see cref="TypeCode"/> for the given <paramref name="type"/>.
    /// </summary>
    [DebuggerStepThrough]
    public static TypeCode GetTypeCode(this Type type)
    {
        if (type == typeof(bool)) { return TypeCode.Boolean; }
        if (type == typeof(char)) { return TypeCode.Char; }
        if (type == typeof(sbyte)) { return TypeCode.SByte; }
        if (type == typeof(byte)) { return TypeCode.Byte; }
        if (type == typeof(short)) { return TypeCode.Int16; }
        if (type == typeof(ushort)) { return TypeCode.UInt16; }
        if (type == typeof(int)) { return TypeCode.Int32; }
        if (type == typeof(uint)) { return TypeCode.UInt32; }
        if (type == typeof(long)) { return TypeCode.Int64; }
        if (type == typeof(ulong)) { return TypeCode.UInt64; }
        if (type == typeof(float)) { return TypeCode.Single; }
        if (type == typeof(double)) { return TypeCode.Double; }
        if (type == typeof(decimal)) { return TypeCode.Decimal; }
        if (type == typeof(DateTime)) { return TypeCode.DateTime; }
        if (type == typeof(string)) { return TypeCode.String; }
        // ReSharper disable once TailRecursiveCall
        // ReSharper disable once ConvertIfStatementToReturnStatement
        if (type.GetTypeInfo().IsEnum) { return Enum.GetUnderlyingType(type).GetTypeCode(); }
        return TypeCode.Object;
    }
        
    public static string AssemblyQualifiedNameWithoutVersion(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (type.AssemblyQualifiedName != null)
        {
            var strArray = type.AssemblyQualifiedName.Split(new char[] { ',' });
            return string.Format("{0}, {1}", strArray[0].Trim(), strArray[1].Trim());
        }

        return null;
    }

    public static bool IsNumericType(this Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.Byte:
            case TypeCode.SByte:
            case TypeCode.UInt16:
            case TypeCode.UInt32:
            case TypeCode.UInt64:
            case TypeCode.Int16:
            case TypeCode.Int32:
            case TypeCode.Int64:
            case TypeCode.Decimal:
            case TypeCode.Double:
            case TypeCode.Single:
                return true;
            case TypeCode.Object:
                if (type.IsNullable(out var innerType))
                {
                    return innerType.IsNumericType();
                }
                return false;
            default:
                return false;
        }
    }

    public static bool IsSequenceType(this Type type)
    {
        if (type == typeof(string))
            return false;

        return type.IsArray || typeof(IEnumerable).IsAssignableFrom(type);
    }

    public static bool IsSequenceType(this Type type, out Type elementType)
    {
        elementType = null;

        if (type == typeof(string))
            return false;

        if (type.IsArray)
        {
            elementType = type.GetElementType();
        }
        else if (type.IsSubClass(typeof(IEnumerable<>), out var implType))
        {
            var genericArgs = implType.GetGenericArguments();
            if (genericArgs.Length == 1)
            {
                elementType = genericArgs[0];
            }
        }

        return elementType != null;
    }

    public static bool IsPredefinedSimpleType(this Type type)
    {
        if ((type.IsPrimitive && (type != typeof(IntPtr))) && (type != typeof(UIntPtr)))
        {
            return true;
        }

        if (type.IsEnum)
        {
            return true;
        }

        return s_predefinedTypes.Any(t => t == type);
    }

    public static bool IsStruct(this Type type)
    {
        if (type.IsValueType)
        {
            return !type.IsPredefinedSimpleType();
        }

        return false;
    }

    public static bool IsPredefinedGenericType(this Type type)
    {
        if (type.IsGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }
        else
        {
            return false;
        }

        return type == typeof(Nullable<>);
    }

    public static bool IsPredefinedType(this Type type)
    {
        if ((!IsPredefinedSimpleType(type) && !IsPredefinedGenericType(type)) && ((type != typeof(byte[]))))
        {
            return (string.Compare(type.FullName, "System.Xml.Linq.XElement", StringComparison.Ordinal) == 0);
        }

        return true;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPlainObjectType(this Type type)
    {
        return type.IsClass && !type.IsSequenceType() && !type.IsPredefinedType();
    }

    public static bool IsInteger(this Type type)
    {
        switch (Type.GetTypeCode(type))
        {
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Gets the underlying type of a <see cref="Nullable{T}" /> type.
    /// </summary>
    public static Type GetNonNullableType(this Type type)
    {
        if (!IsNullable(type, out var wrappedType))
        {
            return type;
        }

        return wrappedType;
    }

    public static bool IsNullable(this Type type, out Type elementType)
    {
        if (type != null && type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            elementType = type.GetGenericArguments()[0];
        else
            elementType = type;

        return elementType != type;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsEnumType(this Type type)
    {
        return type.GetNonNullableType().IsEnum;
    }

    public static bool IsConstructable(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (type.IsAbstract || type.IsInterface || type.IsArray || type.IsGenericTypeDefinition || type == typeof(void))
            return false;

        if (!HasDefaultConstructor(type))
            return false;

        return true;
    }

    [DebuggerStepThrough]
    public static bool IsAnonymous(this Type type)
    {
        if (type.IsGenericType)
        {
            var d = type.GetGenericTypeDefinition();
            if (d.IsClass && d.IsSealed && d.Attributes.HasFlag(TypeAttributes.NotPublic))
            {
                var attributes = d.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false);
                if (attributes != null && attributes.Length > 0)
                {
                    //WOW! We have an anonymous type!!!
                    return true;
                }
            }
        }

        return false;
    }

    [DebuggerStepThrough]
    public static bool HasDefaultConstructor(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (type.IsValueType)
            return true;

        return type.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
            .Any(ctor => ctor.GetParameters().Length == 0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubClass(this Type type, Type check)
    {
        return IsSubClass(type, check, out _);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSubClass(this Type type, Type check, out Type implementingType)
    {
        if (type == null)
            throw new ArgumentNullException(nameof(type));

        if (check == null)
            throw new ArgumentNullException(nameof(check));

        return IsSubClassInternal(type, type, check, out implementingType);
    }

    private static bool IsSubClassInternal(Type initialType, Type currentType, Type check, out Type implementingType)
    {
        if (currentType == check)
        {
            implementingType = currentType;
            return true;
        }

        // don't get interfaces for an interface unless the initial type is an interface
        if (check.IsInterface && (initialType.IsInterface || currentType == initialType))
        {
            foreach (Type t in currentType.GetInterfaces())
            {
                if (IsSubClassInternal(initialType, t, check, out implementingType))
                {
                    // don't return the interface itself, return it's implementor
                    if (check == implementingType)
                        implementingType = currentType;

                    return true;
                }
            }
        }

        if (currentType.IsGenericType && !currentType.IsGenericTypeDefinition)
        {
            if (IsSubClassInternal(initialType, currentType.GetGenericTypeDefinition(), check, out implementingType))
            {
                // INFO: this seems to be wrong! Don't we need the implementing base type (?!?)
                implementingType = currentType;
                return true;
            }
        }

        if (currentType.BaseType == null)
        {
            implementingType = null;
            return false;
        }

        return IsSubClassInternal(initialType, currentType.BaseType, check, out implementingType);
    }

    public static bool IsCompatibleWith(this Type source, Type target)
    {
        if (source == target)
            return true;

        if (!target.IsValueType)
            return target.IsAssignableFrom(source);

        var nonNullableType = source.GetNonNullableType();
        var type = target.GetNonNullableType();

        if ((nonNullableType == source) || (type != target))
        {
            var code = nonNullableType.IsEnum ? TypeCode.Object : Type.GetTypeCode(nonNullableType);
            var code2 = type.IsEnum ? TypeCode.Object : Type.GetTypeCode(type);

            switch (code)
            {
                case TypeCode.SByte:
                    switch (code2)
                    {
                        case TypeCode.SByte:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Byte:
                    switch (code2)
                    {
                        case TypeCode.Byte:
                        case TypeCode.Int16:
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int16:
                    switch (code2)
                    {
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt16:
                    switch (code2)
                    {
                        case TypeCode.UInt16:
                        case TypeCode.Int32:
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int32:
                    switch (code2)
                    {
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt32:
                    switch (code2)
                    {
                        case TypeCode.UInt32:
                        case TypeCode.Int64:
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Int64:
                    switch (code2)
                    {
                        case TypeCode.Int64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.UInt64:
                    switch (code2)
                    {
                        case TypeCode.UInt64:
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            return true;
                    }
                    break;
                case TypeCode.Single:
                    switch (code2)
                    {
                        case TypeCode.Single:
                        case TypeCode.Double:
                            return true;
                    }
                    break;
                default:
                    if (nonNullableType == type)
                    {
                        return true;
                    }
                    break;
            }
        }
        return false;
    }

    public static string GetTypeName(this Type type)
    {
        if (type.IsNullable(out var wrappedType))
        {
            return wrappedType.Name + "?";
        }

        return type.Name;
    }

    /// <summary>
    /// Returns single attribute from the type
    /// </summary>
    /// <typeparam name="TAttribute">Attribute to use</typeparam>
    /// <param name="target">Attribute provider</param>
    ///<param name="inherits"><see cref="MemberInfo.GetCustomAttributes(Type,bool)"/></param>
    /// <returns><em>Null</em> if the attribute is not found</returns>
    /// <exception cref="InvalidOperationException">If there are 2 or more attributes</exception>
    public static TAttribute GetAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
    {
        if (target.IsDefined(typeof(TAttribute), inherits))
        {
            var attributes = target.GetCustomAttributes(typeof(TAttribute), inherits);
            if (attributes.Length > 1)
            {
                throw Errors.MoreThanOneElement();
            }

            return (TAttribute)attributes[0];
        }

        return null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAttribute<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
    {
        return target.IsDefined(typeof(TAttribute), inherits);
    }

    /// <summary>
    /// Given a particular MemberInfo, return the custom attributes of the
    /// given type on that member.
    /// </summary>
    /// <typeparam name="TAttribute">Type of attribute to retrieve.</typeparam>
    /// <param name="target">The member to look at.</param>
    /// <param name="inherits">True to include attributes inherited from base classes.</param>
    /// <returns>Array of found attributes.</returns>
    public static TAttribute[] GetAttributes<TAttribute>(this ICustomAttributeProvider target, bool inherits) where TAttribute : Attribute
    {
        if (target.IsDefined(typeof(TAttribute), inherits))
        {
            var attributes = target
                .GetCustomAttributes(typeof(TAttribute), inherits)
                .Cast<TAttribute>();

            return SortAttributesIfPossible(attributes).ToArray();
        }

        return new TAttribute[0];
    }

    /// <summary>
    /// Given a particular MemberInfo, find all the attributes that apply to this
    /// member. Specifically, it returns the attributes on the type, then (if it's a
    /// property accessor) on the property, then on the member itself.
    /// </summary>
    /// <typeparam name="TAttribute">Type of attribute to retrieve.</typeparam>
    /// <param name="member">The member to look at.</param>
    /// <param name="inherits">true to include attributes inherited from base classes.</param>
    /// <returns>Array of found attributes.</returns>
    public static TAttribute[] GetAllAttributes<TAttribute>(this MemberInfo member, bool inherits)
        where TAttribute : Attribute
    {
        List<TAttribute> attributes = new List<TAttribute>();

        if (member.DeclaringType != null)
        {
            attributes.AddRange(GetAttributes<TAttribute>(member.DeclaringType, inherits));

            MethodBase methodBase = member as MethodBase;
            if (methodBase != null)
            {
                PropertyInfo prop = GetPropertyFromMethod(methodBase);
                if (prop != null)
                {
                    attributes.AddRange(GetAttributes<TAttribute>(prop, inherits));
                }
            }
        }

        attributes.AddRange(GetAttributes<TAttribute>(member, inherits));
        return attributes.ToArray();
    }

    [SuppressMessage("ReSharper", "SuspiciousTypeConversion.Global")]
    internal static IEnumerable<TAttribute> SortAttributesIfPossible<TAttribute>(IEnumerable<TAttribute> attributes)
        where TAttribute : Attribute
    {
        if (typeof(IOrdered).IsAssignableFrom(typeof(TAttribute)))
        {
            return attributes.Cast<IOrdered>().OrderBy(x => x.Ordinal).Cast<TAttribute>();
        }

        return attributes;
    }

    /// <summary>
    /// Given a MethodBase for a property's get or set method,
    /// return the corresponding property info.
    /// </summary>
    /// <param name="method">MethodBase for the property's get or set method.</param>
    /// <returns>PropertyInfo for the property, or null if method is not part of a property.</returns>
    public static PropertyInfo GetPropertyFromMethod(this MethodBase method)
    {
        Guard.AgainstNull(method, "method");

        PropertyInfo property = null;
        if (method.IsSpecialName)
        {
            Type containingType = method.DeclaringType;
            if (containingType != null)
            {
                if (method.Name.StartsWith("get_", StringComparison.InvariantCulture) ||
                    method.Name.StartsWith("set_", StringComparison.InvariantCulture))
                {
                    string propertyName = method.Name.Substring(4);
                    property = containingType.GetProperty(propertyName);
                }
            }
        }
        return property;
    }

    internal static Type FindIEnumerable(this Type seqType)
    {
        if (seqType == null || seqType == typeof(string))
            return null;

        if (seqType.IsArray)
            return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());

        if (seqType.IsGenericType)
        {
            var args = seqType.GetGenericArguments();
            foreach (var arg in args)
            {
                var ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                if (ienum.IsAssignableFrom(seqType))
                    return ienum;
            }
        }

        Type[] ifaces = seqType.GetInterfaces();
        if (ifaces.Length > 0)
        {
            foreach (Type iface in ifaces)
            {
                Type ienum = FindIEnumerable(iface);
                if (ienum != null)
                    return ienum;
            }
        }

        if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            return FindIEnumerable(seqType.BaseType);

        return null;
    }
        
}

/// <summary>
/// Enum representing the possible types of a sequence.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum SequenceType
{
    /// <summary>
    /// Represents an invalid type.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// Represents a <see cref="String"/>.
    /// </summary>
    String,

    /// <summary>
    /// Represents an Array.
    /// </summary>
    Array,

    /// <summary>
    /// Represents a <see cref="BitArray"/>. This type is non generic.
    /// </summary>
    BitArray,

    /// <summary>
    /// Represents an <see cref="ArrayList"/>. This type is non generic.
    /// </summary>
    ArrayList,

    /// <summary>
    /// Represents a <see cref="Queue"/>. This type is non generic.
    /// </summary>
    Queue,

    /// <summary>
    /// Represents a <see cref="Stack"/>. This type is non generic.
    /// </summary>
    Stack,

    /// <summary>
    /// Represents a <see cref="Hashtable"/>. This type is non generic.
    /// </summary>
    Hashtable,

    /// <summary>
    /// Represents a <see cref="SortedList"/>. This type is non generic.
    /// </summary>
    SortedList,

    /// <summary>
    /// Represents a <see cref="Dictionary"/>. This type is non generic.
    /// </summary>
    Dictionary,

    /// <summary>
    /// Represents a <see cref="ListDictionary"/>. This type is non generic.
    /// </summary>
    ListDictionary,

    /// <summary>
    /// Represents an <see cref="IList"/>. This interface type is non generic.
    /// </summary>
    IList,

    /// <summary>
    /// Represents an <see cref="ICollection"/>. This interface type is non generic.
    /// </summary>
    ICollection,

    /// <summary>
    /// Represents an <see cref="IDictionary"/>. This interface type is non generic.
    /// </summary>
    IDictionary,

    /// <summary>
    /// Represents an <see cref="IEnumerable"/>. This interface type is non generic.
    /// </summary>
    IEnumerable,

    /// <summary>
    /// Represents a custom implementation of <see cref="IEnumerable"/>.
    /// </summary>
    Custom,

    /// <summary>
    /// Represents a <see cref="List{T}"/>.
    /// </summary>
    GenericList,

    /// <summary>
    /// Represents a <see cref="LinkedList{T}"/>.
    /// </summary>
    GenericLinkedList,

    /// <summary>
    /// Represents a <see cref="Collection{T}"/>.
    /// </summary>
    GenericCollection,

    /// <summary>
    /// Represents a <see cref="Queue{T}"/>.
    /// </summary>
    GenericQueue,

    /// <summary>
    /// Represents a <see cref="Stack{T}"/>.
    /// </summary>
    GenericStack,

    /// <summary>
    /// Represents a <see cref="HashSet{T}"/>.
    /// </summary>
    GenericHashSet,

    /// <summary>
    /// Represents a <see cref="SortedList{TKey,TValue}"/>.
    /// </summary>
    GenericSortedList,

    /// <summary>
    /// Represents a <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    GenericDictionary,

    /// <summary>
    /// Represents a <see cref="SortedDictionary{TKey, TValue}"/>.
    /// </summary>
    GenericSortedDictionary,

    /// <summary>
    /// Represents a <see cref="BlockingCollection{T}"/>.
    /// </summary>
    GenericBlockingCollection,

    /// <summary>
    /// Represents a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
    /// </summary>
    GenericConcurrentDictionary,

    /// <summary>
    /// Represents a <see cref="ConcurrentBag{T}"/>.
    /// </summary>
    GenericConcurrentBag,

    /// <summary>
    /// Represents an <see cref="IList{T}"/>.
    /// </summary>
    GenericIList,

    /// <summary>
    /// Represents an <see cref="ICollection{T}"/>.
    /// </summary>
    GenericICollection,

    /// <summary>
    /// Represents an <see cref="IEnumerable{T}"/>.
    /// </summary>
    GenericIEnumerable,

    /// <summary>
    /// Represents an <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    GenericIDictionary,

    /// <summary>
    /// Represents an <see> <cref>ICollection{KeyValuePair{TKey, TValue}}</cref></see>.
    /// </summary>
    GenericICollectionKeyValue,

    /// <summary>
    /// Represents an <see> <cref>IEnumerable{KeyValuePair{TKey, TValue}}</cref></see>.
    /// </summary>
    GenericIEnumerableKeyValue,

    /// <summary>
    /// Represents a custom implementation of <see cref="IEnumerable{T}"/>.
    /// </summary>
    GenericCustom
}