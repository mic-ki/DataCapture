using Microsoft.EntityFrameworkCore;

namespace Application.Models;

public class PagedList<T>
{
    public List<T> Items { get; }
    public int PageNumber { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }
    public int PageSize { get; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        PageSize = pageSize;
        Items = items;
    }

    public static async Task<PagedList<T>> CreateAsync<TSource>(
        IQueryable<TSource> source, 
        int pageNumber, 
        int pageSize, 
        Func<TSource, T> map,
        CancellationToken cancellationToken = default)
    {
        var count = await source.CountAsync(cancellationToken);
        
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var mappedItems = items.Select(map).ToList();
        
        return new PagedList<T>(mappedItems, count, pageNumber, pageSize);
    }
}
