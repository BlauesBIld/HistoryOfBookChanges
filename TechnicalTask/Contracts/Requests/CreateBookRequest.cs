using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechnicalTask.Contracts.Requests;

public sealed record CreateBookRequest(
    [property: Required, MaxLength(200)] string Title,
    [property: MaxLength(4000)] string? Description,
    DateTime PublishDate,
    List<AuthorRequest>? Authors
);