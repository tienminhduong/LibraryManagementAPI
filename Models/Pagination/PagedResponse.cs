using Microsoft.EntityFrameworkCore;

namespace LibraryManagementAPI.Models.Pagination;

public class PagedResponse<T>(int pageNumber, int pageSize, IEnumerable<T> data, int totalItems)
{
    public IEnumerable<T> Data { get; set; } = data;
    public int PageNumber { get; set; } = pageNumber;
    public int PageSize { get; set; } = pageSize;
    public int TotalItems { get; set; } = totalItems;
    public int TotalPages { get; set; } = pageSize > 0 ? (int)Math.Ceiling(totalItems / (double)pageSize) : 0;

    public static async Task<PagedResponse<T>> FromQueryable(IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber < 1) pageNumber = 1;
        if (pageSize < 1) pageSize = 10;

        var total = source.Count();
        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResponse<T>(pageNumber, pageSize, items, total);
    }
}