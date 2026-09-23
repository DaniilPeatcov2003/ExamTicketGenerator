using ClosedXML.Excel;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<JournalStore>();

var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/tickets", (JournalStore journal) => Results.Ok(journal.GetEntries()));

app.MapPost("/api/tickets", (TicketRequest request, JournalStore journal) =>
{
    if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName))
    {
        return Results.BadRequest(new { message = "Введите фамилию и имя." });
    }

    try
    {
        var entry = journal.AddEntry(request.LastName.Trim(), request.FirstName.Trim());
        return Results.Ok(entry);
    }
    catch (IOException)
    {
        return Results.Problem("Закройте journal.xlsx в Excel и повторите попытку.", statusCode: 409);
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Problem("Нет доступа к journal.xlsx.", statusCode: 403);
    }
});

app.Run();

public sealed record TicketRequest(string? LastName, string? FirstName);

public sealed record TicketEntry(string LastName, string FirstName, int TicketNumber, DateTime CreatedAt);

public sealed class JournalStore
{
    private readonly string fileName = Environment.GetEnvironmentVariable("JOURNAL_FILE") ?? "journal.xlsx";
    private static readonly Random Random = new();
    private readonly object syncRoot = new();

    public TicketEntry AddEntry(string lastName, string firstName)
    {
        var entry = new TicketEntry(lastName, firstName, Random.Next(1, 21), DateTime.Now);

        lock (syncRoot)
        {
            using var workbook = File.Exists(fileName) ? new XLWorkbook(fileName) : new XLWorkbook();
            var worksheet = workbook.Worksheets.Count == 0
                ? workbook.Worksheets.Add("Journal")
                : workbook.Worksheet(1);

            if (worksheet.Cell("A1").IsEmpty())
            {
                worksheet.Cell("A1").Value = "Last name";
                worksheet.Cell("B1").Value = "First name";
                worksheet.Cell("C1").Value = "Номер билета";
                worksheet.Cell("D1").Value = "Дата и время";
                worksheet.Row(1).Style.Font.Bold = true;
            }

            var row = worksheet.LastRowUsed()?.RowNumber() + 1 ?? 2;
            worksheet.Cell(row, 1).Value = entry.LastName;
            worksheet.Cell(row, 2).Value = entry.FirstName;
            worksheet.Cell(row, 3).Value = entry.TicketNumber;
            worksheet.Cell(row, 4).Value = entry.CreatedAt;
            worksheet.Cell(row, 4).Style.DateFormat.Format = "dd.MM.yyyy HH:mm:ss";
            worksheet.Columns("A:D").AdjustToContents();
            workbook.SaveAs(fileName);
        }

        return entry;
    }

    public IReadOnlyList<TicketEntry> GetEntries()
    {
        if (!File.Exists(fileName))
        {
            return [];
        }

        lock (syncRoot)
        {
            using var workbook = new XLWorkbook(fileName);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet is null || worksheet.LastRowUsed()?.RowNumber() is not int lastRow || lastRow < 2)
            {
                return [];
            }

            var entries = new List<TicketEntry>();
            for (var row = 2; row <= lastRow; row++)
            {
                entries.Add(new TicketEntry(
                    worksheet.Cell(row, 1).GetString(),
                    worksheet.Cell(row, 2).GetString(),
                    worksheet.Cell(row, 3).GetValue<int>(),
                    worksheet.Cell(row, 4).GetDateTime()));
            }

            return entries;
        }
    }
}