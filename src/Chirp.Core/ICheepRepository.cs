namespace Chirp.Core;

public interface ICheepRepository {
    public List<CheepDTO> GetCheeps(int page);
    public List<CheepDTO> GetCheepsFromAuthor(string author);
    public List<CheepDTO> GetCheepsFromAuthorPage(string author, int page);
    public List<CheepDTO> GetCheepsFromAuthorPageEmail(string email, int page);

    public Cheep AddCheep(Author author, string text);

    public List<CheepDTO> GetCheepsFromAuthorPages(List<string> authors, int page);
    public List<CheepDTO> GetCheepsFromAuthorEmail(string email);
    public List<Cheep> GetCheep(string userEmail, int cheepId);
    public void DeleteCheep(Cheep cheep);
    public void AddLike(Likes likes);
    public bool IsLiked(string user, int CheepId);
    public void UnLike(string user, int CheepId);
    public int LikeCount(int CheepId);
    public int TotalLikeCountUser(string username);
    public int AmountOfCheeps();
    public void UnBlock(string userEmail, string blockEmail);
    public bool UserBlockedSomeone(string userEmail);
    public List<CheepDTO> GetCheepsNotBlocked(string userEmail);
    public List<CheepDTO> GetLiked(string user);
    public Cheep GetCheepFromId(int cheepId);

}