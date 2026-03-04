namespace TechnicalTask.Contracts.Responses;

public sealed record AuthorResponse(
    int Id,
    string FirstName,
    string LastName,
    DateTime? BirthDate
);