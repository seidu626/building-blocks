// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.IPagedList`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections;

namespace BuildingBlocks.Types;

public interface IPagedList<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
{
    int PageIndex { get; }

    int PageSize { get; }

    int TotalCount { get; }

    int TotalPages { get; }

    bool HasPreviousPage { get; }

    bool HasNextPage { get; }
}