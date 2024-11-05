using System.Linq.Expressions;
using System.Reflection;

namespace BuildingBlocks.Extensions
{
    public static class ExpressionExtensions
    {
        public static string GetPropertyName<TInstance, TProperty>(
            this Expression<Func<TInstance, TProperty>> selector)
        {
            if (selector.Body is MemberExpression body)
                return body.Member.Name;

            if (selector.Body is UnaryExpression unaryExpression && unaryExpression.Operand is MemberExpression operand)
                return operand.Member.Name;

            throw new ArgumentException("Invalid expression. The selector must refer to a property.");
        }

        public static PropertyInfo GetProperty<TInstance, TProperty>(
            this Expression<Func<TInstance, TProperty>> selector,
            TInstance instance)
        {
            if (!(selector.Body is MemberExpression body))
                throw new ArgumentException($"Expression '{selector}' refers to a method, not a property.");

            if (!(body.Member is PropertyInfo propertyInfo))
                throw new ArgumentException($"Expression '{selector}' refers to a field, not a property.");

            if (!propertyInfo.ReflectedType.IsAssignableFrom(typeof(TInstance)))
                throw new ArgumentException(
                    $"Expression '{selector}' refers to a property that is not from type {typeof(TInstance)}.");

            return propertyInfo;
        }
    }
}