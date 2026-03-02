namespace TechnicalTask.Entities;

public class Author
{
    protected Author()
    {
    }

    public Author(Guid id, string firstName, string lastName, DateTime birthDate, List<Book> books)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Books = books;
    }

    public Guid Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime BirthDate { get; set; }

    public List<Book> Books { get; set; }
}