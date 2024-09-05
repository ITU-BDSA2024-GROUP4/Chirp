using System.Globalization;
using Chrip.CLI;
using CsvHelper;
using SimpleDB;

var csvDatabase = new CSVDatabase<Cheep>();
if(args[0] == "read"){
    IEnumerable<Cheep> cheeps;

    try
    {
        if (args.Length > 1) {
            cheeps = csvDatabase.Read(int.Parse(args[1]));
        } else {
            cheeps = csvDatabase.Read();
        }
        UserInterface.cheepPrinter(cheeps);
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
} else if (args[0] == "cheep")
{   
    string author = Environment.UserName;
    DateTimeOffset timestamp = DateTime.UtcNow;
    csvDatabase.Store(new Cheep(author, args[1], timestamp.ToUnixTimeSeconds()));
    
    UserInterface.cheep(false);
}
else
{
    UserInterface.cheep(true);
}
public record Cheep(string Author, string Message, long Timestamp);