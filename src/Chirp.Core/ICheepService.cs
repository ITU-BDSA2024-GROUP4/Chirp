namespace Chirp.Core;

public interface ICheepService 
{
    ICheepRepository repository { get; set; }
    public IAuthorRepository TEMP { get; set; }
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public void DeleteCheep(string userEmail, int cheepId);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    public void AddCheep(string email, string message);

    public bool IsFollowingUserName(string username, string author);
    public List<AuthorDTO> GetFollowers(string email);
    public List<CheepDTO> GetOwnTimeline(string userEmail);
    public List<CheepDTO> GetOwnTimelinePage(string userEmail, int page);
    public int GetFollowerCount(string email);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);
    public void ForgetMe(string email);
    public void CreateLike(string user, int CheepId);
    public bool IsLiked(string user, int CheepId);
    public void UnLike(string user, int CheepId);
    public int LikeCount(int CheepId);
    public int TotalLikeCountUser(string username);
    public int AmountOfCheeps();
    public List<CheepDTO> GetLiked(string user);
    public void UnBlock(string userEmail, string blockEmail);
    public bool UserBlockedSomeone(string userEmail);
    public List<CheepDTO> GetCheepsNotBlocked(string userEmail);
    public int GetTotalCheeps(string email);
}