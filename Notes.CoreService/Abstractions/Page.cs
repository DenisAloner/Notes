namespace Notes.CoreService.Abstractions;

public class Page<T>
{
    /// <summary>
    /// Список
    /// </summary>
    public IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Номер страницы
    /// </summary>
    public int CurrentPage { get; }

    /// <summary>
    /// Всего страниц
    /// </summary>
    public int TotalPages { get; }

    /// <summary>
    /// Размер страницы
    /// </summary>
    public int PageSize { get; init; }

    /// <summary>
    /// Всего записей
    /// </summary>
    public int TotalCount { get; init; }

    /// <summary>
    /// Есть ли предыдущая страница
    /// </summary>
    public bool HasPrevious => CurrentPage > 1;

    /// <summary>
    /// Есть ли следующая страница
    /// </summary>
    public bool HasNext => CurrentPage < TotalPages;

    public Page(IReadOnlyList<T> items, int count, int pageNumber, int pageSize)
    {
        Items = items;
        TotalCount = count;
        PageSize = pageSize;
        CurrentPage = pageNumber;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }
}