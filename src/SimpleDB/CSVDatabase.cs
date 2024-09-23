namespace SimpleDB;

using System.Globalization;
using CsvHelper;
using System;
using System.IO;
using System.Text.RegularExpressions;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    string FilePath;
    private static CSVDatabase<T> instance = null;
    private static readonly object padlock = new object();

    public CSVDatabase() {    
        
        FilePath = Directory.GetCurrentDirectory();
        string pattern = @"[^/]+$";
        Regex regex = new Regex(pattern);
        Match match = regex.Match(FilePath);
        
        while (match.Value != "Chirp") 
        {
            FilePath = System.IO.Directory.GetParent(FilePath).FullName;
            match = regex.Match(FilePath);
        }
        
        FilePath += "/data/chirp_cli_db.csv"; //Here: the path from the root of the project to the data file
    }
    public static CSVDatabase<T> Instance 
    {
        get 
        {
            lock(padlock) 
            {
                if (instance == null) 
                {
                    instance = new CSVDatabase<T>();
                }
                return instance;
            }
        }
    }
    public IEnumerable<T> Read(int? limit = null) 
    {
        try
        {
            using StreamReader reader = new StreamReader(FilePath);
            using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            {
                List<T> csvList = csvReader.GetRecords<T>().ToList();

                if (limit == null) 
                {
                    return csvList;
                } else {
                    List<T> limitedCsvList = new List<T>();
                    
                    for (int i = csvList.Count - 1; i >= csvList.Count - limit; i--) 
                    {
                        limitedCsvList.Add(csvList[i]);
                    }
                    
                    return limitedCsvList;
                }
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);
            
            return Enumerable.Empty<T>();
        }
    }
    public record Cheep(string Author, string Message, long Timestamp);
    
    public void Store(T record)
    {
        using StreamWriter db = new StreamWriter(FilePath, true);
        using CsvWriter csvWriter = new CsvWriter(db, CultureInfo.InvariantCulture);
        {
            string author = Environment.UserName;
            DateTimeOffset timestamp = DateTime.UtcNow;

            csvWriter.NextRecord();
            csvWriter.WriteRecord(record);
        }
    }
}