// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.ExpressionExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Extensions;

public static class ExpressionExtensions
{
  public static string GetPropertyName<TInstance, TProperty>(
    this Expression<Func<TInstance, TProperty>> selector)
  {
    return (selector.Body is MemberExpression body ? body.Member.Name : (string) null) ?? ((MemberExpression) ((UnaryExpression) selector.Body).Operand).Member.Name;
  }

  public static PropertyInfo GetProperty<TInstance, TProperty>(
    this Expression<Func<TInstance, TProperty>> selector,
    TInstance instance)
  {
    Type c = typeof (TInstance);
    if (!(selector.Body is MemberExpression body))
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(49, 1);
      interpolatedStringHandler.AppendLiteral("Expression '");
      interpolatedStringHandler.AppendFormatted<Expression<Func<TInstance, TProperty>>>(selector);
      interpolatedStringHandler.AppendLiteral("' refers to a method, not a property.");
      throw new ArgumentException(interpolatedStringHandler.ToStringAndClear());
    }
    if (!(body.Member is PropertyInfo member))
    {
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
      interpolatedStringHandler.AppendLiteral("Expression '");
      interpolatedStringHandler.AppendFormatted<Expression<Func<TInstance, TProperty>>>(selector);
      interpolatedStringHandler.AppendLiteral("' refers to a field, not a property.");
      throw new ArgumentException(interpolatedStringHandler.ToStringAndClear());
    }
    if (((MemberInfo) member).ReflectedType.IsAssignableFrom(c))
      return member;
    DefaultInterpolatedStringHandler interpolatedStringHandler1 = new DefaultInterpolatedStringHandler(58, 2);
    interpolatedStringHandler1.AppendLiteral("Expression '");
    interpolatedStringHandler1.AppendFormatted<Expression<Func<TInstance, TProperty>>>(selector);
    interpolatedStringHandler1.AppendLiteral("' refers to a property that is not from type ");
    interpolatedStringHandler1.AppendFormatted<Type>(c);
    interpolatedStringHandler1.AppendLiteral(".");
    throw new ArgumentException(interpolatedStringHandler1.ToStringAndClear());
  }
}