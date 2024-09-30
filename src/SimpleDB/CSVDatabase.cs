using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;

namespace SimpleDB;

public sealed class CSVDatabase<T> : IDatabaseRepository<T>
{
    private readonly string FilePath;

    private CSVDatabase()
    {
        FilePath = "chirp_cli_db.csv";
    }

    public static CSVDatabase<T> Instance { get; } = new();

    public IEnumerable<T> Read(int? limit = null)
    {
        try
        {
            using StreamReader reader = new(FilePath);
            using CsvReader csvReader = new(reader, CultureInfo.InvariantCulture);
            {
                List<T> csvList = csvReader.GetRecords<T>().ToList();

                if (limit == null)
                {
                    return csvList;
                }

                List<T> limitedCsvList = new();

                for (int i = csvList.Count - 1; i >= csvList.Count - limit; i--)
                {
                    limitedCsvList.Add(csvList[i]);
                }

                return limitedCsvList;
            }
        }
        catch (IOException e)
        {
            Console.WriteLine("The file could not be read:");
            Console.WriteLine(e.Message);

            return Enumerable.Empty<T>();
        }
    }

    public void Store(T record)
    {
        using StreamWriter db = new(FilePath, true);
        using CsvWriter csvWriter = new(db, CultureInfo.InvariantCulture);
        {
            string author = Environment.UserName;
            DateTimeOffset timestamp = DateTime.UtcNow;

            csvWriter.NextRecord();
            csvWriter.WriteRecord(record);
        }
    }

    public record Cheep(string Author, string Message, long Timestamp);
}