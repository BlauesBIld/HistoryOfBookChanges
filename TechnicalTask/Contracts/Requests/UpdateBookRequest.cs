using System.ComponentModel.DataAnnotations;

namespace TechnicalTask.Contracts.Requests;

public sealed record UpdateBookRequest(
    [MaxLength(200)] string? Title,
    [MaxLength(4000)] string? Description,
    DateTime? PublishDate,
    List<AuthorRequest>? Authors
);