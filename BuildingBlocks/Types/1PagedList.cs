// Decompiled with JetBrains decompiler
// Type: BuildingBlocks.Types.PagedList`1
// Assembly: BuildingBlocks, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8587CF18-BE3D-4726-89FF-4AF7AAC01FA5
// Assembly location: C:\Users\420919\Repositories\STS\Api\BuildingBlocks.dll

#nullable disable
using System.Collections;

namespace BuildingBlocks.Types;

[Serializable]
public class PagedList<T> :
    List<T>,
    IPagedList<T>,
    IList<T>,
    ICollection<T>,
    IEnumerable<T>,
    IEnumerable
{
    public PagedList()
    {
    }

    public PagedList(IList<T> source, int pageIndex, int pageSize, int? totalCount = null)
    {
        pageSize = Math.Max(pageSize, 1);
        this.TotalCount = totalCount ?? source.Count;
        this.TotalPages = this.TotalCount / pageSize;
        if (this.TotalCount % pageSize > 0)
            this.TotalPages = this.TotalPages + 1;
        this.PageSize = pageSize;
        this.PageIndex = pageIndex;
        this.AddRange(totalCount.HasValue
            ? (IEnumerable<T>)source
            : source.Skip<T>(pageIndex * pageSize).Take<T>(pageSize));
    }

    public int PageIndex { get; }

    public int PageSize { get; }

    public int TotalCount { get; }

    public int TotalPages { get; }

    public bool HasPreviousPage => this.PageIndex > 1;

    public bool HasNextPage => this.PageIndex + 1 < this.TotalPages;
}