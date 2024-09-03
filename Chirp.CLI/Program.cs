using System.Text.RegularExpressions;

if(args[0] == "read"){
    try
    {
        using StreamReader reader = new("chirp_cli_db.csv");
        reader.ReadLine();
        String line;
        
        while ((line = reader.ReadLine()) != null)
        {
            var match = Parse(line);
            
            var user = match.Groups["user"].Value;
            var message = match.Groups["message"].Value;
            var timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(match.Groups["timestamp"].Value)).ToLocalTime();
            Console.WriteLine($"{user} @ {timestamp.DateTime.ToString("MM\\/dd\\/yy HH:mm:ss")} - {message}");

        }
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
} else if (args[0] == "cheep")
{
    using (StreamWriter db = new StreamWriter("chirp_cli_db.csv", true))
    {
        string name = Environment.UserName;
        DateTimeOffset timestamp = DateTime.UtcNow;
        db.WriteLine(name + ",\"" + args[1] + "\"," + timestamp.ToUnixTimeSeconds());
    }
    Console.WriteLine("Cheeped!");
}
else
{
    Console.WriteLine("Command not recognized!");
}


Match Parse(string value)
{
    var pattern = @"^(?<user>[A-Za-z\-_0-9]+),[""](?<message>[\w\s\S]+)[""],(?<timestamp>[\d]+)$";
    var rg = new Regex(pattern);
    var match = rg.Match(value);
    return match;
}