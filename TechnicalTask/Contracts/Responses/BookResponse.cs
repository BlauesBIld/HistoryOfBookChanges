namespace TechnicalTask.Contracts.Responses;

public sealed record BookResponse(
    string Uid,
    string Title,
    string? Description,
    DateTime PublishDate,
    List<AuthorResponse> Authors
);