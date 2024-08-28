// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Extensions.CollectionSlicer
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
namespace BuildingBlocks.Extensions;

public static class CollectionSlicer
{
  public static IEnumerable<IEnumerable<T>> Slice<T>(
    this IEnumerable<T> source,
    params int[] sizes)
  {
    return ((IEnumerable<int>) sizes).Any<int>((Func<int, bool>) (step => step != 0)) ? new Slicer<T>(source.GetEnumerator(), sizes).Slice() : throw new InvalidOperationException("Can't slice a collection with step length 0.");
  }
}