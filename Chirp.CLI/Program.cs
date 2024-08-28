if(args[0] == "read"){
    try
    {
        // Open the text file using a stream reader.
        using StreamReader reader = new("chirp_cli_db.csv");
        reader.ReadLine();
        String line;
        
        while ((line = reader.ReadLine()) != null)
        {
            string[] values = line.Split('"');
            string name = values[0].Remove(values[0].Length - 1);
            DateTimeOffset timestamp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(values[2].Remove(0, 1))).ToLocalTime();
            string msg = values[1];
            Console.WriteLine(name + " @ " + timestamp.DateTime + ": " + msg);
        }
    }
    catch (IOException e)
    {
        Console.WriteLine("The file could not be read:");
        Console.WriteLine(e.Message);
    }
}

