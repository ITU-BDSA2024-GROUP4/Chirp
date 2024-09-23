using CsvHelper;
using SimpleDB;
using DocoptNet;

namespace Chirp.CLI;

public class Program {
    
    

    const string usage = @"Chirp CLI version.

Usage:
  chirp read [<limit>]
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

    public static void Main(string[] args)
    {
        var arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;
        var csvDatabase = new CSVDatabase<Cheep>();

        if (arguments["read"].IsTrue)
        {
            try
            {
                IEnumerable<Cheep> cheeps;
                int limit = arguments["<limit>"].AsInt;

                if (limit >= 1)
                {
                    cheeps = csvDatabase.Read(limit);
                }
                else
                {
                    cheeps = csvDatabase.Read();
                }
                var UI = new UserInterface();
                UI.PrintCheeps(cheeps);
            }
            catch (IOException e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }
        else if (arguments["cheep"].IsTrue)
        {
            string author = Environment.UserName;
            DateTimeOffset timestamp = DateTime.UtcNow;
            string message = arguments["<message>"].ToString();

            csvDatabase.Store(new Cheep(author, message, timestamp.ToUnixTimeSeconds()));

            Console.WriteLine("Cheeped!");
        }
        else
        {
            Console.WriteLine("Command not recognized!");
        }
    }
}
public record Cheep(string Author, string Message, long Timestamp);