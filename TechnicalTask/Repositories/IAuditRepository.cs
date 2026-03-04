using TechnicalTask.Common;
using TechnicalTask.Entities;

namespace TechnicalTask.Repositories;

public sealed record AuditQuery(
    Guid? BookId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Search,
    string? ChangeType,
    int Page,
    int PageSize,
    SortDirection SortDirection
);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount
);

public interface IAuditRepository
{
    Task AddAsync(Audit entry, CancellationToken ct = default);
    Task<PagedResult<Audit>> QueryAsync(AuditQuery query, CancellationToken ct = default);
}