namespace Garimpo.Application.Dtos;

/// <summary>Envelope paginado para listagens da API.</summary>
public sealed record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage)
{
    public static PagedResultDto<T> Create(IReadOnlyList<T> items, int page, int pageSize, int totalCount)
    {
        int totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;

        return new PagedResultDto<T>(
            items,
            page,
            pageSize,
            totalCount,
            totalPages,
            page < totalPages);
    }
}
