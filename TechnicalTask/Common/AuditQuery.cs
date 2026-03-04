using TechnicalTask.Entities;

namespace TechnicalTask.Common;

public sealed record AuditQuery(
    Guid? BookId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Search,
    BookChangeType? ChangeType,
    int Page,
    int PageSize,
    SortDirection SortDirection,
    AuditGroupBy GroupBy
);