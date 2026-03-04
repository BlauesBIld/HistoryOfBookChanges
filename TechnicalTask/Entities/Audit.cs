namespace TechnicalTask.Entities;

public enum BookChangeType
{
    Created,
    Updated,
    Deleted
}

public class Audit
{
    public Audit()
    {
    }

    public Audit(Guid id, Guid bookId, BookChangeType changeType, DateTimeOffset changedAt, string fieldName, string? oldValue, string? newValue, string description)
    {
        Id = id;
        BookId = bookId;
        ChangeType = changeType;
        ChangedAt = changedAt;
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        Description = description;
    }

    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public BookChangeType ChangeType { get; set; }
    public DateTimeOffset ChangedAt { get; set; }
    public string FieldName { get; set; } = null!;
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string Description { get; set; } = null!;
}