namespace TechnicalTask.Contracts.Responses;

public sealed record BookResponse(
    Guid Id,
    string Title,
    string? Description,
    DateTime PublishDate,
    List<AuthorResponse> Authors
);