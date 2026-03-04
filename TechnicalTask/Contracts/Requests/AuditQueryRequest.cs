using TechnicalTask.Common;

namespace TechnicalTask.Contracts.Requests;

public sealed record AuditQueryRequest(
    Guid? BookId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Search,
    string? ChangeType,
    int Page = 1,
    int PageSize = 20,
    SortDirection SortDirection = SortDirection.Descending
);