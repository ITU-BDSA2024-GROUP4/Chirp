namespace Chirp.Core;

public interface IAuthorService
{
    public void AddAuthor(string name, string email);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public AuthorDTO GetAuthor(string email);
    public AuthorDTO GetAuthorUserName(string userName);
    public AuthorDTO GetOrCreateAuthor(string name, string email);
    public bool IsFollowing(string email, string authorEmail);
    public void CreateFollow(string username, string user, string follow);
    public void CreateBlock(string userEmail, string blockemail);
    public bool IsBlocked(string userEmail, string blockEmail);
    public void UnFollow(string user, string unfollow);
}