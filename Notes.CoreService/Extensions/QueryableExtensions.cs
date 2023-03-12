using Microsoft.EntityFrameworkCore;
using Notes.CoreService.Abstractions;

namespace Notes.CoreService.Extensions;

public static class QueryableExtensions
{
    public static async Task<Page<T>> ToPageAsync<T>(
        this IQueryable<T> source,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var count = await source.CountAsync(cancellationToken);

        var items = await source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new Page<T>(items, count, pageNumber, pageSize);
    }
}