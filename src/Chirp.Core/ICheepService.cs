namespace Chirp.Core;

public interface ICheepService 
{
    ICheepRepository repository { get; set; }
    
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);

    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    public AuthorDTO GetAuthor(string email);

    public AuthorDTO GetOrCreateAuthor(string name, string email);
    public void CreateCheep(string email, string message);

    public void CreateAuthor(string name, string email);
    public void CreateFollow(string username, string user, string follow);
    public void UnFollow(string user, string unfollow);
    public bool IsFollowing(string user, string author);
    public List<AuthorDTO> GetFollowers(string email);
    public List<CheepDTO> GetOwnTimeline(string userEmail);
    public List<CheepDTO> GetOwnTimelinePage(string userEmail, int page);
    public void ForgetMe(string email);
    public void CreateLike(string user, int CheepId);
    public bool IsLiked(string user, int CheepId);
    public void UnLike(string user, int CheepId);
    public int LikeCount(int CheepId);
    public int AmountOfCheeps();
}