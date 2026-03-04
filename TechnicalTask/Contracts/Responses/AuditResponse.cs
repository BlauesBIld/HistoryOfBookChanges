namespace TechnicalTask.Contracts.Responses;

public sealed record AuditResponse(
    Guid Id,
    Guid BookId,
    string ChangeType,
    DateTimeOffset ChangedAt,
    string FieldName,
    string? OldValue,
    string? NewValue,
    string Description
);