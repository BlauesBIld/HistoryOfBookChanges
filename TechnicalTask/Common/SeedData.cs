using TechnicalTask.Contracts.Requests;
using TechnicalTask.Services;

namespace TechnicalTask.Common;

public static class SeedData
{
    private static readonly string[] Titles =
    {
        "Midnight Railway", "Cryptic Timetables", "Fog Over Platform 9", "Signals in the Dark",
        "The Last Conductor", "Rust & Steam", "Tickets to Nowhere", "Echoes Between Stations",
        "The Quiet Carriage", "Blueprints of a Lost Line", "The Iron Schedule", "Northbound Secrets"
    };

    private static readonly string[] Descriptions =
    {
        "A story about routes, rules, and the odd things that happen between stations.",
        "Collected notes from long journeys and quiet platforms.",
        "An investigation into missing trains and stranger passengers.",
        "A technical guide disguised as a travel diary.",
        "Short chapters, long shadows. Something is always just out of sight.",
        "A calm read that becomes unsettling the longer you stay on board."
    };

    private static readonly string[] FirstNames =
    {
        "Anna", "Lukas", "Mina", "Jakob", "Elena", "Noah", "Lea", "David", "Sofia", "Felix",
        "Mara", "Jonas", "Nora", "Simon", "Emilia", "Max"
    };

    private static readonly string[] LastNames =
    {
        "Bauer", "Huber", "Wagner", "Mayer", "Schmidt", "Fischer", "Weber", "Klein",
        "Novak", "Koller", "Gruber", "Zimmermann", "Leitner", "Steiner"
    };

    public static async Task InitializeAsync(IServiceProvider services, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var bookService = scope.ServiceProvider.GetRequiredService<IBookService>();

        var rng = new Random(1337);

        var totalBooksToCreate = 500;
        var maxUpdatesPerBook = 4;
        var deleteChancePercent = 12;

        for (var i = 0; i < totalBooksToCreate; i++)
        {
            ct.ThrowIfCancellationRequested();

            var createRequest = CreateRandomBookRequest(rng, i);
            var created = await bookService.CreateAsync(createRequest, ct);

            var updates = rng.Next(0, maxUpdatesPerBook + 1);
            for (var u = 0; u < updates; u++)
            {
                ct.ThrowIfCancellationRequested();

                var updateRequest = CreateRandomUpdateRequest(rng, created.Title);
                await bookService.UpdateAsync(created.Id, updateRequest, ct);
            }

            if (rng.Next(0, 100) < deleteChancePercent)
            {
                ct.ThrowIfCancellationRequested();
                await bookService.DeleteAsync(created.Id, ct);
            }
        }
    }

    private static CreateBookRequest CreateRandomBookRequest(Random rng, int index)
    {
        var title = $"{Pick(rng, Titles)} #{index + 1}";
        var description = Pick(rng, Descriptions);
        var publishDate = RandomDate(rng, 1950, 2024);

        var authorCount = rng.Next(1, 4);
        var authors = new List<AuthorRequest>(authorCount);

        for (var i = 0; i < authorCount; i++)
        {
            var first = Pick(rng, FirstNames);
            var last = Pick(rng, LastNames);
            var birth = RandomDate(rng, 1940, 2002);

            authors.Add(new AuthorRequest(first, last, birth));
        }

        return new CreateBookRequest(title, description, publishDate, authors);
    }

    private static UpdateBookRequest CreateRandomUpdateRequest(Random rng, string? currentTitle)
    {
        string? title = null;
        string? description = null;
        DateTime? publishDate = null;
        List<AuthorRequest>? authors = null;

        var updateMask = rng.Next(1, 16);

        if ((updateMask & 1) != 0)
            title = $"{currentTitle} (Rev {rng.Next(2, 10)})";

        if ((updateMask & 2) != 0)
            description = Pick(rng, Descriptions);

        if ((updateMask & 4) != 0)
            publishDate = RandomDate(rng, 1950, 2024);

        if ((updateMask & 8) != 0)
        {
            var authorCount = rng.Next(1, 4);
            authors = new List<AuthorRequest>(authorCount);

            for (var i = 0; i < authorCount; i++)
            {
                var first = Pick(rng, FirstNames);
                var last = Pick(rng, LastNames);
                var birth = RandomDate(rng, 1940, 2002);
                authors.Add(new AuthorRequest(first, last, birth));
            }
        }

        return new UpdateBookRequest(title, description, publishDate, authors);
    }

    private static DateTime RandomDate(Random rng, int yearMinInclusive, int yearMaxInclusive)
    {
        var year = rng.Next(yearMinInclusive, yearMaxInclusive + 1);
        var month = rng.Next(1, 13);
        var day = rng.Next(1, DateTime.DaysInMonth(year, month) + 1);
        return new DateTime(year, month, day);
    }

    private static string Pick(Random rng, string[] values)
    {
        return values[rng.Next(values.Length)];
    }
}