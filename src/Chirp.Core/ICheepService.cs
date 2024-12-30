namespace Chirp.Core;

public interface ICheepService
{
    ICheepRepository repository { get; set; }
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public void RemoveCheep(string username, int cheepId);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    public void AddCheep(string username, string message);
    public List<CheepDTO> GetOwnTimeline(string username);
    public List<CheepDTO> GetOwnTimelinePage(string username, int page);
    public void CreateLike(string username, int cheepId);
    public bool IsLiked(string username, int CheepId);
    public void UnLike(string username, int CheepId);
    public int LikeCount(int CheepId);
    public int TotalLikeCountUser(string username);
    public int AmountOfCheeps();
    public List<CheepDTO> GetLiked(string username);
    public void UnBlock(string userEmail, string blockEmail);
    public bool UserBlockedSomeone(string username);
    public List<CheepDTO> GetCheepsNotBlocked(string username);
    public int GetTotalCheeps(string email);
}