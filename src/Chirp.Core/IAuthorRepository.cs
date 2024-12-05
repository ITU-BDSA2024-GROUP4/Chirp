namespace Chirp.Core;

public interface IAuthorRepository
{
    public List<Author> GetAuthor(string email);
    public Author AddAuthor(string name, string email);
    public List<Author> GetAuthorUserName(string userName);
    public List<AuthorDTO> GetFollowers(string email);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public bool IsFollowing(string email, string authorEmail);
    public void AddFollow(string user, string following);
    public void UnFollow(string user, string unfollowing);
    public bool IsBlocked(string userEmail, string blockEmail);
    public void CreateBlock(string userEmail, string blockemail);

}