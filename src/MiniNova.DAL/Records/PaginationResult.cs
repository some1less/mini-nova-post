namespace MiniNova.DAL.Records;

public record PaginationResult<T>(IEnumerable<T> Items, int TotalCount);