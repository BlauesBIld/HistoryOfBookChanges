namespace TechnicalTask.Contracts.Responses;

public sealed record AuditResponse(
    string BookUid,
    DateTime ChangedAtUtc,
    string Description,
    string? ChangeType
);