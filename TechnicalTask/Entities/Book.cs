namespace TechnicalTask.Entities;

public class Book
{
    protected Book()
    {
    }

    public Book(Guid id, string title, string? description, DateTime publishDate, List<Author>? authors)
    {
        Id = id;
        Title = title;
        Description = description;
        PublishDate = publishDate;
        Authors = authors ?? new List<Author>();
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public DateTime PublishDate { get; set; }
    public List<Author> Authors { get; set; }
}