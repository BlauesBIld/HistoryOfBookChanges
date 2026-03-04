namespace TechnicalTask.Contracts.Responses;

public sealed record AuditGroupResponse<T>(
    string Key,
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount
);