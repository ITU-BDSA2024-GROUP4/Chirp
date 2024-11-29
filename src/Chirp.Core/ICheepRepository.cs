namespace Chirp.Core;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    
    public List<CheepDTO> GetCheepsFromAuthorPageEmail(string email, int page);

    public Author CreateAuthor(string name, string email);

    public Cheep CreateCheep(Author author, string text);

    public List<Author> GetAuthor(string email);
    public void CreateFollow(string user, string following);
    public void UnFollow(string user, string unfollowing);
    public bool IsFollowing(string user, string author);
    public List<AuthorDTO> GetFollowers(string email);
    public List<CheepDTO> GetCheepsFromAuthorPages(List<string> authors, int page);
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email);
    public void CreateLike(string user, int CheepId);
    public void ForgetUser(string email);
    public bool IsLiked(string user, int CheepId);
    public void UnLike(string user, int CheepId);
    public int LikeCount(int CheepId);
    public int AmountOfCheeps();
    public List<CheepDTO> GetLiked(string user);

}