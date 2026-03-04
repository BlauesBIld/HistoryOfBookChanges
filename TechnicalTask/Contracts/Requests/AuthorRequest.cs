using System.ComponentModel.DataAnnotations;

namespace TechnicalTask.Contracts.Requests;

public sealed record AuthorRequest(
    [property: Required, MaxLength(100)] string FirstName,
    [property: Required, MaxLength(100)] string LastName,
    DateTime? BirthDate
);