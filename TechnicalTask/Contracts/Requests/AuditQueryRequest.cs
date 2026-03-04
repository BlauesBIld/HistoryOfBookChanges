namespace TechnicalTask.Contracts.Requests;

public enum SortDirection
{
    Ascending,
    Descending
}

public sealed record AuditQueryRequest(
    string? BookUid,
    DateTime? FromUtc,
    DateTime? ToUtc,
    string? Search,
    string? ChangeType,
    int Page = 1,
    int PageSize = 20,
    SortDirection SortDirection = SortDirection.Descending
);