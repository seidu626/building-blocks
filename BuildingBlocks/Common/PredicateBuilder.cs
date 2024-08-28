// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Common.PredicateBuilder
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Linq.Expressions;

namespace BuildingBlocks.Common;

public static class PredicateBuilder
{
  public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate)
  {
    return predicate;
  }

  public static Expression<Func<T, bool>> True<T>()
  {
    return (Expression<Func<T, bool>>) (param => true);
  }

  public static Expression<Func<T, bool>> False<T>()
  {
    return (Expression<Func<T, bool>>) (param => false);
  }

  public static Expression<Func<T, bool>> Or<T>(
    this Expression<Func<T, bool>> expr1,
    Expression<Func<T, bool>> expr2)
  {
    InvocationExpression right = Expression.Invoke((Expression) expr2, (IEnumerable<Expression>) expr1.Parameters);
    return Expression.Lambda<Func<T, bool>>((Expression) Expression.OrElse(expr1.Body, (Expression) right), (IEnumerable<ParameterExpression>) expr1.Parameters);
  }

  public static Expression<Func<T, bool>> And<T>(
    this Expression<Func<T, bool>> expr1,
    Expression<Func<T, bool>> expr2)
  {
    InvocationExpression right = Expression.Invoke((Expression) expr2, (IEnumerable<Expression>) expr1.Parameters);
    return Expression.Lambda<Func<T, bool>>((Expression) Expression.AndAlso(expr1.Body, (Expression) right), (IEnumerable<ParameterExpression>) expr1.Parameters);
  }

  public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
  {
    return Expression.Lambda<Func<T, bool>>((Expression) Expression.Not(expression.Body), (IEnumerable<ParameterExpression>) expression.Parameters);
  }
}