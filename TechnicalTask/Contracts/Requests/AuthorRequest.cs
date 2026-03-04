using System.ComponentModel.DataAnnotations;

namespace TechnicalTask.Contracts.Requests;

public sealed record AuthorRequest(
    [Required] [MaxLength(100)] string FirstName,
    [Required] [MaxLength(100)] string LastName,
    DateTime? BirthDate
);