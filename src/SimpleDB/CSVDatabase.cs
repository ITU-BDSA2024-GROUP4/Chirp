namespace SimpleDB;

using System.ComponentModel.Design;
using System.Globalization;
using CsvHelper;
public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    const string FILE = "../../chirp_cli_db.csv";
    public IEnumerable<T> Read(int? limit = null) 
    {
        try
        {
            using StreamReader reader = new StreamReader(FILE);
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
        using StreamWriter db = new StreamWriter(FILE, true);
        using CsvWriter csvWriter = new CsvWriter(db, CultureInfo.InvariantCulture);
        {
            string author = Environment.UserName;
            DateTimeOffset timestamp = DateTime.UtcNow;

            csvWriter.NextRecord();
            csvWriter.WriteRecord(record);
        }
    }
}

