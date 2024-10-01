using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DocoptNet;

namespace Chirp.CLI;

public class Program
{
    //public const string BASEURL = "https://bdsagroup4chirpremotedb.azurewebsites.net/";
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
                List<Cheep> cheeps;
                int limit = arguments["<limit>"].AsInt;

                cheeps = await ReadCheeps(client, limit);

                UserInterface UI = new();
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
            string message = arguments["<message>"].ToString();
            
            await WriteCheep(client, author, message, DateTime.Now);

            Console.WriteLine("Cheeped!");
        }
        else
        {
            Console.WriteLine("Command not recognized!");
        }
    }

    public static async Task<List<Cheep>> ReadCheeps(HttpClient client, int? limit = null)
    {
        try
        {
            using HttpResponseMessage response = await client.GetAsync(BASEURL + "/cheeps");
            string responseString = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseString);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
           
                List<Cheep> cheeps = JsonSerializer.Deserialize<List<Cheep>>(responseString, options);
            
            if (limit > 0)
            {
                return cheeps.TakeLast(limit.Value).ToList();
                
            }
            
            
            return cheeps;
        
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e.Message);
            return null;
        }
    }
    
    public static async Task WriteCheep(HttpClient client, string author, string message, DateTimeOffset timestamp)
    {
        try
        {
            Cheep cheep = new Cheep(author, message, timestamp.ToUnixTimeSeconds());

            JsonContent jsonCheep = JsonContent.Create(cheep);
            
            await client.PostAsync(BASEURL + "/cheep", jsonCheep);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}

public record Cheep(string Author, string Message, long Timestamp);