using TechnicalTask.Contracts.Requests;
using TechnicalTask.Contracts.Responses;
using TechnicalTask.Entities;
using TechnicalTask.Repositories;
using TechnicalTask.Services.Interfaces;

namespace TechnicalTask.Services;

public sealed class BookService : IBookService
{
    private const string EntityName = "Book";
    private const string FieldTitle = "Title";
    private const string FieldDescription = "Description";
    private const string FieldPublishDate = "PublishDate";
    private const string FieldAuthors = "Authors";

    private readonly IAuditRepository _audits;
    private readonly IBookRepository _books;

    public BookService(IBookRepository books, IAuditRepository audits)
    {
        _books = books;
        _audits = audits;
    }

    public async Task<BookResponse> CreateAsync(CreateBookRequest request, CancellationToken ct = default)
    {
        var bookId = Guid.NewGuid();

        var authors = (request.Authors ?? new List<AuthorRequest>())
            .Select(MapNewAuthor)
            .ToList();

        var book = new Book(bookId, request.Title, request.Description, request.PublishDate, authors);

        LinkBookToAuthors(book);

        await _books.AddAsync(book, ct);
        await AddAuditAsync(AuditFor(book.Id, BookChangeType.Created, DateTimeOffset.UtcNow, EntityName, null, null, "Book was created"), ct);

        return MapBook(book);
    }

    public async Task<BookResponse?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        return book is null ? null : MapBook(book);
    }

    public async Task<BookResponse?> UpdateAsync(Guid id, UpdateBookRequest request, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            return null;

        var changedAt = DateTimeOffset.UtcNow;

        await ApplyScalarChangesAsync(book, request, changedAt, ct);

        if (request.Authors is not null)
            await ApplyAuthorChangesAsync(book, request.Authors, changedAt, ct);

        var updated = await _books.UpdateAsync(book, ct);
        return updated ? MapBook(book) : null;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null)
            return false;

        var removed = await _books.DeleteAsync(id, ct);
        if (!removed)
            return false;

        await AddAuditAsync(AuditFor(id, BookChangeType.Deleted, DateTimeOffset.UtcNow, EntityName, null, null, "Book was deleted"), ct);
        return true;
    }

    private async Task ApplyScalarChangesAsync(Book book, UpdateBookRequest request, DateTimeOffset changedAt, CancellationToken ct)
    {
        if (TrySet(book.Title, request.Title, out var newTitle))
        {
            await AddFieldAuditAsync(book.Id, BookChangeType.Updated, changedAt, FieldTitle, book.Title, newTitle,
                $"Title was changed to \"{newTitle}\"", ct);
            book.Title = newTitle;
        }

        if (TrySet(book.Description, request.Description, out var newDescription))
        {
            await AddFieldAuditAsync(book.Id, BookChangeType.Updated, changedAt, FieldDescription, book.Description, newDescription,
                "Description was changed", ct);
            book.Description = newDescription;
        }

        if (request.PublishDate is not null && request.PublishDate.Value != book.PublishDate)
        {
            var oldValue = book.PublishDate.ToString("yyyy-MM-dd");
            var newValue = request.PublishDate.Value.ToString("yyyy-MM-dd");

            await AddFieldAuditAsync(book.Id, BookChangeType.Updated, changedAt, FieldPublishDate, oldValue, newValue,
                $"Publish date was changed to {newValue}", ct);

            book.PublishDate = request.PublishDate.Value;
        }
    }

    private async Task ApplyAuthorChangesAsync(Book book, List<AuthorRequest> incomingRequests, DateTimeOffset changedAt, CancellationToken ct)
    {
        var currentKeys = book.Authors.Select(AuthorKey.From).ToHashSet();
        var incomingKeys = incomingRequests.Select(AuthorKey.From).ToHashSet();

        var keysToAdd = incomingKeys.Except(currentKeys).ToList();
        var keysToRemove = currentKeys.Except(incomingKeys).ToList();

        if (keysToAdd.Count == 0 && keysToRemove.Count == 0)
            return;

        var incomingByKey = incomingRequests
            .GroupBy(AuthorKey.From)
            .ToDictionary(g => g.Key, g => g.First());

        foreach (var key in keysToAdd)
        {
            var req = incomingByKey[key];
            var newAuthor = MapNewAuthor(req);

            newAuthor.Books.Add(book);
            book.Authors.Add(newAuthor);

            await AddFieldAuditAsync(book.Id, BookChangeType.Updated, changedAt, FieldAuthors, null, key.Display,
                $"Author {newAuthor.FirstName} {newAuthor.LastName} was added", ct);
        }

        foreach (var key in keysToRemove)
        {
            var existing = book.Authors.FirstOrDefault(a => AuthorKey.From(a).Equals(key));
            if (existing is null)
                continue;

            book.Authors.Remove(existing);
            existing.Books.Remove(book);

            await AddFieldAuditAsync(book.Id, BookChangeType.Updated, changedAt, FieldAuthors, key.Display, null,
                $"Author {existing.FirstName} {existing.LastName} was removed", ct);
        }
    }

    private static Author MapNewAuthor(AuthorRequest request)
    {
        return new Author(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.BirthDate,
            new List<Book>()
        );
    }

    private static void LinkBookToAuthors(Book book)
    {
        foreach (var author in book.Authors)
            author.Books.Add(book);
    }

    private async Task AddFieldAuditAsync(
        Guid bookId,
        BookChangeType changeType,
        DateTimeOffset changedAt,
        string fieldName,
        string? oldValue,
        string? newValue,
        string description,
        CancellationToken ct)
    {
        await AddAuditAsync(AuditFor(bookId, changeType, changedAt, fieldName, oldValue, newValue, description), ct);
    }

    private async Task AddAuditAsync(Audit audit, CancellationToken ct)
    {
        await _audits.AddAsync(audit, ct);
    }

    private static Audit AuditFor(
        Guid bookId,
        BookChangeType changeType,
        DateTimeOffset changedAt,
        string fieldName,
        string? oldValue,
        string? newValue,
        string description)
    {
        return new Audit(
            Guid.NewGuid(),
            bookId,
            changeType,
            changedAt,
            fieldName,
            oldValue,
            newValue,
            description
        );
    }

    private static bool TrySet(string current, string? incoming, out string newValue)
    {
        if (incoming is null || incoming == current)
        {
            newValue = current;
            return false;
        }

        newValue = incoming;
        return true;
    }

    private static BookResponse MapBook(Book book)
    {
        var authors = book.Authors
            .Select(a => new AuthorResponse(a.Id, a.FirstName, a.LastName, a.BirthDate))
            .ToList();

        return new BookResponse(book.Id, book.Title, book.Description, book.PublishDate, authors);
    }

    private readonly record struct AuthorKey(string FirstLowerTrim, string LastLowerTrim, DateTime? BirthDate)
    {
        public string Display =>
            $"{Cap(FirstLowerTrim)} {Cap(LastLowerTrim)} ({BirthDate:yyyy-MM-dd})";

        public static AuthorKey From(AuthorRequest a)
        {
            return new AuthorKey(a.FirstName.Trim().ToLowerInvariant(), a.LastName.Trim().ToLowerInvariant(), a.BirthDate);
        }

        public static AuthorKey From(Author a)
        {
            return new AuthorKey(a.FirstName.Trim().ToLowerInvariant(), a.LastName.Trim().ToLowerInvariant(), a.BirthDate);
        }

        private static string Cap(string s)
        {
            return s.Length == 0 ? s : char.ToUpperInvariant(s[0]) + s[1..];
        }
    }
}