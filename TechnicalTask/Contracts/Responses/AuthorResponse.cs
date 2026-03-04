namespace TechnicalTask.Contracts.Responses;

public sealed record AuthorResponse(
    Guid Id,
    string FirstName,
    string LastName,
    DateTime? BirthDate
);