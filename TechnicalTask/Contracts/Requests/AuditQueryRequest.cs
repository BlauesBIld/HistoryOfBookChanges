using TechnicalTask.Common;
using TechnicalTask.Entities;

namespace TechnicalTask.Contracts.Requests;

public sealed record AuditQueryRequest(
    Guid? BookId,
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Search,
    BookChangeType? ChangeType,
    int Page = 1,
    int PageSize = 20,
    SortDirection SortDirection = SortDirection.Descending,
    AuditGroupBy GroupBy = AuditGroupBy.None,
    int GroupPage = 1,
    int GroupPageSize = 20
);