using System.Globalization;
using CsvHelper;
using SimpleDB;


if(args[0] == "read"){
    IDatabaseRepository<Cheep> database;

    try
    {
        using StreamReader reader = new StreamReader("chirp_cli_db.csv");
        using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        {
            DateTimeOffset timestamp;
            foreach (Cheep cheep in csvReader.GetRecords<Cheep>())
            {
                timestamp = DateTimeOffset.FromUnixTimeSeconds(cheep.Timestamp).ToLocalTime();
                Console.WriteLine(cheep.Author + " @ " + timestamp.DateTime + ": " + cheep.Message);
            }
        }
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
} else if (args[0] == "cheep")
{
    using StreamWriter db = new StreamWriter("chirp_cli_db.csv", true);
    using CsvWriter csvWriter = new CsvWriter(db, CultureInfo.InvariantCulture);
    {
        string author = Environment.UserName;
        DateTimeOffset timestamp = DateTime.UtcNow;
        
        csvWriter.NextRecord();
        csvWriter.WriteRecord(new Cheep(author, args[1], timestamp.ToUnixTimeSeconds()));
    }
    Console.WriteLine("Cheeped!");
}
else
{
    Console.WriteLine("Command not recognized!");
}

public record Cheep(string Author, string Message, long Timestamp);