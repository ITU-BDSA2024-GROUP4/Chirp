namespace SimpleDB;

public interface IDatabaseRepository<T>
{
    //public IEnumerable<T> Read(int? limit = null);
    public IEnumerable<T> Read(string path);
    public void Store(T record);
}
