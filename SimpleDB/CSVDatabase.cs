namespace SimpleDB;
using System.Globalization;
using CsvHelper;
sealed class CSVDatabase<T> : IDatabaseRepository<T>

{
    public IEnumerable<T> Read(string path) 
    {
        try
        {
            
            using StreamReader reader = new StreamReader("../chirp_cli_db.csv");
            using CsvReader csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
            {
                return csvReader.GetRecords<T>().ToList();
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
        
    }
}

