// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.LinqExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace BuildingBlocks.Extensions;

public static class LinqExtensions
{
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static PropertyInfo ExtractPropertyInfo(this LambdaExpression propertyAccessor)
  {
    return propertyAccessor.ExtractMemberInfo() as PropertyInfo;
  }

  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static FieldInfo ExtractFieldInfo(this LambdaExpression propertyAccessor)
  {
    return propertyAccessor.ExtractMemberInfo() as FieldInfo;
  }

  public static MemberInfo ExtractMemberInfo(this LambdaExpression propertyAccessor)
  {
    if (propertyAccessor == null)
      throw new ArgumentNullException(nameof (propertyAccessor));
    try
    {
      LambdaExpression lambdaExpression = propertyAccessor;
      return (!(lambdaExpression.Body is UnaryExpression body) ? (MemberExpression) lambdaExpression.Body : (MemberExpression) body.Operand).Member;
    }
    catch (Exception ex)
    {
      throw new ArgumentException("The property or field accessor expression is not in the expected format 'o => o.PropertyOrField'.", ex);
    }
  }
}