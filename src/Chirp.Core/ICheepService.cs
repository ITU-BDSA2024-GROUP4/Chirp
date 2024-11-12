namespace Chirp.Core;

public interface ICheepService 
{
    ICheepRepository repository { get; set; }
    
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);

    public Author GetOrCreateAuthor(string name, string email);
    public void CreateCheep(Author author, string message);
}