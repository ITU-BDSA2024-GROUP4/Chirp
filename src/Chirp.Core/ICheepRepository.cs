namespace Chirp.Core;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
    
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email, int page);

    public Author CreateAuthor(string name, string email);

    public Cheep CreateCheep(Author author, string text);

    public List<Author> GetAuthor(string email);
}