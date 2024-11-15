namespace Chirp.Core;

public interface ICheepService 
{
    ICheepRepository repository { get; set; }
    
    public List<CheepDTO> GetCheeps(int page);

    public List<CheepDTO> GetCheepsFromAuthor(string author, int page);
    public AuthorDTO GetAuthor(string email);

    public AuthorDTO GetOrCreateAuthor(string name, string email);
    public void CreateCheep(string email, string message);
    public void CreateFollow(string user, string follow);
    public void UnFollow(string user, string unfollow);
    public bool IsFollowing(string user, string author);
}