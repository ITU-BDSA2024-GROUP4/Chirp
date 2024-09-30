using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

app.MapPost("/cheep", (Cheep cheep) => CSVDatabase<Cheep>.Instance.Store(cheep));

app.MapGet("/cheeps", () => CSVDatabase<Cheep>.Instance.Read());


app.Run();

public record Cheep(string Author, string Message, long Timestamp);