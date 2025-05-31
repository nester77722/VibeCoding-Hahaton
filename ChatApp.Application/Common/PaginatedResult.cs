namespace ChatApp.Application.Common;

public record PaginatedResult<T>(
    List<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages
);