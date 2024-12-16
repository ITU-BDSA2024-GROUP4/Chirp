namespace Chirp.Core;

public interface ICheepRepository
{
    // Methods for retrieving cheeps
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    public List<CheepDTO> GetCheepsFromAuthorPages(List<string> authors, int page);
    public List<CheepDTO> GetCheepsFromAuthorPageEmail(string email, int page);
    public List<Cheep> GetCheep(string username, int cheepId);

    public List<CheepDTO> GetCheepsNotBlocked(string username);
    public List<CheepDTO> GetLiked(string username);
    public List<Cheep> GetCheepFromId(int cheepId);

    // Methods for adding and removing cheeps
    public Cheep AddCheep(Cheep cheep, Author author);
    public void RemoveCheep(Cheep cheep);

    // Methods for handling likes
    public void AddLike(Likes likes);
    public bool IsLiked(string username, int CheepId);
    public void UnLike(Likes like);
    public int LikeCount(int CheepId);
    public int TotalLikeCountUser(string username);
    public List<Likes> GetLike(string username, int cheepId);

    // Methods for counting cheeps
    public int AmountOfCheeps();
    public int CheepCount();

    // Methods for blocking/unblocking users
    public void UnBlock(string userEmail, string blockEmail);
    public bool UserBlockedSomeone(string username);
}