using SimpleDB;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();


app.MapPost("/cheep",
    () => CSVDatabase<Cheep>.Instance.Store(new Cheep("The Rizzler", "SUP GIRL",
        DateTimeOffset.UtcNow.ToUnixTimeSeconds())));

app.MapGet("/cheeps", () => CSVDatabase<Cheep>.Instance.Read());


app.Run();

public record Cheep(string Author, string Message, long Timestamp);