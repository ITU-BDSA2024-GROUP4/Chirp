namespace Chirp.Core;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
    
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email, int page);

    public Author CreateAuthor(string name, string email);

    public Cheep CreateCheep(Author author, string text);

    public List<Author> GetAuthor(string email);
    public void CreateFollow(string user, string following);
    public void UnFollow(string user, string unfollowing);
    public bool IsFollowing(string user, string author);

}