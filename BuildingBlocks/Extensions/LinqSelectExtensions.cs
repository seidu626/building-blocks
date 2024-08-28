// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.LinqSelectExtensions
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable enable
namespace BuildingBlocks.Extensions;

public static class LinqSelectExtensions
{
  public static 
#nullable disable
    IEnumerable<TSource> DistinctBy<TSource, TKey>(
      this IEnumerable<TSource> source,
      Func<TSource, TKey> keySelector)
  {
    HashSet<TKey> seenKeys = new HashSet<TKey>();
    foreach (TSource source1 in source)
    {
      if (seenKeys.Add(keySelector(source1)))
        yield return source1;
    }
  }

  public static IEnumerable<T> Page<T>(this IEnumerable<T> en, int pageSize, int page)
  {
    return en.Skip<T>(page * pageSize).Take<T>(pageSize);
  }

  public static IQueryable<T> Page<T>(this IQueryable<T> en, int pageSize, int page)
  {
    return Queryable.Take<T>(Queryable.Skip<T>(en, page * pageSize), pageSize);
  }

  public static IEnumerable<LinqSelectExtensions.SelectTryResult<TSource, TResult>> SelectTry<TSource, TResult>(
    this IEnumerable<TSource> enumerable,
    Func<TSource, TResult> selector)
  {
    foreach (TSource source in enumerable)
    {
      LinqSelectExtensions.SelectTryResult<TSource, TResult> selectTryResult;
      try
      {
        selectTryResult = new LinqSelectExtensions.SelectTryResult<TSource, TResult>(source, selector(source), (Exception) null);
      }
      catch (Exception ex)
      {
        selectTryResult = new LinqSelectExtensions.SelectTryResult<TSource, TResult>(source, default (TResult), ex);
      }
      yield return selectTryResult;
    }
  }

  public static IEnumerable<TResult> OnCaughtException<TSource, TResult>(
    this IEnumerable<LinqSelectExtensions.SelectTryResult<TSource, TResult>> enumerable,
    Func<Exception, TResult> exceptionHandler)
  {
    return enumerable.Select<LinqSelectExtensions.SelectTryResult<TSource, TResult>, TResult>((Func<LinqSelectExtensions.SelectTryResult<TSource, TResult>, TResult>) (x => x.CaughtException != null ? exceptionHandler(x.CaughtException) : x.Result));
  }

  public static IEnumerable<TResult> OnCaughtException<TSource, TResult>(
    this IEnumerable<LinqSelectExtensions.SelectTryResult<TSource, TResult>> enumerable,
    Func<TSource, Exception, TResult> exceptionHandler)
  {
    return enumerable.Select<LinqSelectExtensions.SelectTryResult<TSource, TResult>, TResult>((Func<LinqSelectExtensions.SelectTryResult<TSource, TResult>, TResult>) (x => x.CaughtException != null ? exceptionHandler(x.Source, x.CaughtException) : x.Result));
  }

  public class SelectTryResult<TSource, TResult>
  {
    internal SelectTryResult(TSource source, TResult result, Exception exception)
    {
      this.Source = source;
      this.Result = result;
      this.CaughtException = exception;
    }

    public TSource Source { get; private set; }

    public TResult Result { get; private set; }

    public Exception CaughtException { get; private set; }
  }
}