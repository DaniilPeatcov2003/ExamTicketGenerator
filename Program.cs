using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TicketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<JournalStore>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<TicketDbContext>().Database.EnsureCreated();
}

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/tickets", async (TicketDbContext database) =>
    Results.Ok(await database.Tickets.AsNoTracking().OrderBy(ticket => ticket.CreatedAt).ToListAsync()));

app.MapPost("/api/tickets", async (TicketRequest request, TicketDbContext database, JournalStore journal) =>
{
    if (string.IsNullOrWhiteSpace(request.LastName) || string.IsNullOrWhiteSpace(request.FirstName))
    {
        return Results.BadRequest(new { message = "Введите фамилию и имя." });
    }

    var entry = journal.CreateEntry(request.LastName.Trim(), request.FirstName.Trim());
    database.Tickets.Add(entry);
    await database.SaveChangesAsync();

        string? warning = null;
        try
        {
            journal.AppendToExcel(entry);
        }
        catch (IOException)
        {
            warning = "Билет сохранён в SQL Server, но journal.xlsx сейчас открыт или недоступен.";
        }
        catch (UnauthorizedAccessException)
        {
            warning = "Билет сохранён в SQL Server, но нет доступа к journal.xlsx.";
        }

    return Results.Ok(new
    {
        entry.Id,
        entry.LastName,
        entry.FirstName,
        entry.TicketNumber,
        entry.CreatedAt,
        warning
    });
});

app.Run();

public sealed record TicketRequest(string? LastName, string? FirstName);

public sealed class TicketEntry
{
    public int Id { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int TicketNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}

public sealed class TicketDbContext(DbContextOptions<TicketDbContext> options) : DbContext(options)
{
    public DbSet<TicketEntry> Tickets => Set<TicketEntry>();
}

public sealed class JournalStore
{
    private readonly string fileName = Environment.GetEnvironmentVariable("JOURNAL_FILE") ?? "journal.xlsx";
    private static readonly Random Random = new();
    private readonly object syncRoot = new();

    public TicketEntry CreateEntry(string lastName, string firstName)
    {
        return new TicketEntry
        {
            LastName = lastName,
            FirstName = firstName,
            TicketNumber = Random.Next(1, 21),
            CreatedAt = DateTime.Now
        };
    }

    public void AppendToExcel(TicketEntry entry)
    {
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
    }

}