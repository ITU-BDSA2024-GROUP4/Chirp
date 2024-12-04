namespace Chirp.Core;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    
    public List<CheepDTO> GetCheepsFromAuthorPageEmail(string email, int page);

    public Author CreateAuthor(string name, string email);

    public Cheep CreateCheep(Author author, string text);
    public List<Cheep> GetOwnCheeps(string userEmail);

    public List<Author> GetAuthor(string email);
    public List<Author> GetAuthorUserName(string userName);
    public void CreateFollow(string user, string following);
    public void UnFollow(string user, string unfollowing);
    public bool IsFollowing(string email, string authorEmail);
    public bool IsFollowingUserName(string username, string author);
    public List<AuthorDTO> GetFollowers(string email);
    public int GetFollowerCount(string email);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);

    public List<CheepDTO> GetCheepsFromAuthorPages(List<string> authors, int page);
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email);
    public void CreateLike(string user, int CheepId);
    public void ForgetUser(string email);
    public bool IsLiked(string user, int CheepId);
    public void UnLike(string user, int CheepId);
    public int LikeCount(int CheepId);
    public int TotalLikeCountUser(string username);
    public int AmountOfCheeps();
    public void CreateBlock(string userEmail, string blockemail);
    public void UnBlock(string userEmail, string blockEmail);
    public bool IsBlocked(string userEmail, string blockEmail);
    public bool UserBlockedSomeone(string userEmail);
    public List<CheepDTO> GetCheepsNotBlocked(string userEmail);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public List<CheepDTO> GetLiked(string user);

}