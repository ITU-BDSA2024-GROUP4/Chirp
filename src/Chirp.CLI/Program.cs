using System.Net.Http.Headers;
using System.Text.Json.Nodes;
using DocoptNet;


using SimpleDB;

namespace Chirp.CLI;

public class Program
{
    public const string BASEURL = "http://localhost:5141";

    private const string usage = @"Chirp CLI version.

Usage:
  chirp read [<limit>]
  chirp cheep <message>
  chirp (-h | --help)
  chirp --version

Options:
  -h --help     Show this screen.
  --version     Show version.
";

    public static async Task Main(string[] args)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.Accept.Clear();
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        client.BaseAddress = new Uri(BASEURL);

        IDictionary<string, ValueObject>? arguments = new Docopt().Apply(usage, args, version: "1.0", exit: true)!;

        if (arguments["read"].IsTrue)
        {
            try
            {
                Task<IEnumerable<Cheep>> cheeps;
                int limit = arguments["<limit>"].AsInt;

                if (limit >= 1)
                {
                    //cheeps = CSVDatabase<Cheep>.Instance.Read(limit);
                }
                else
                {
                    //cheeps = CSVDatabase<Cheep>.Instance.Read();
                    await Task.Run(async () =>
                    {
                        await ReadCheeps(client, limit);
                    });
                }

                UserInterface UI = new();
                //UI.PrintCheeps(cheeps);
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

            CSVDatabase<Cheep>.Instance.Store(new Cheep(author, message, timestamp.ToUnixTimeSeconds()));

            Console.WriteLine("Cheeped!");
        }
        else
        {
            Console.WriteLine("Command not recognized!");
        }
    }

    public static async Task ReadCheeps(HttpClient client, int? limit = null)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(BASEURL + "/cheeps");
            string responseString = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonObject.Parse(responseString);
            Console.WriteLine(jsonResponse[2]);
            
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}

public record Cheep(string Author, string Message, long Timestamp);