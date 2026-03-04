namespace TechnicalTask.Contracts.Responses;

public sealed record AuditEntryResponse(
    string BookUid,
    DateTime ChangedAtUtc,
    string Description,
    string? ChangeType
);