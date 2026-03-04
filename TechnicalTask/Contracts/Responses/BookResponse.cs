namespace TechnicalTask.Contracts.Responses;

public sealed record BookResponse(
    Guid Uid,
    string Title,
    string? Description,
    DateTime PublishDate,
    List<AuthorResponse> Authors
);