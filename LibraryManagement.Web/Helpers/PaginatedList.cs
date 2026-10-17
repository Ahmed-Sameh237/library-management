using Microsoft.EntityFrameworkCore;

namespace LibraryManagement.Web.Helpers;

public class PaginatedList<T> : List<T>
{
    public int PageIndex { get; }
    public int TotalPages { get; }
    public int TotalCount { get; }

    public bool HasPrevious => PageIndex > 1;
    public bool HasNext => PageIndex < TotalPages;

    private PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        TotalCount = count;
        PageIndex = pageIndex;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        AddRange(items);
    }

    public static async Task<PaginatedList<T>> CreateAsync(
        IQueryable<T> source, int pageIndex, int pageSize)
    {
        var count = await source.CountAsync();   // query 1: how many rows match?

        var totalPages = (int)Math.Ceiling(count / (double)pageSize);
        pageIndex = Math.Max(1, Math.Min(pageIndex, Math.Max(1, totalPages)));

        var items = await source                 // query 2: just this page
            .Skip((pageIndex - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PaginatedList<T>(items, count, pageIndex, pageSize);
    }
}