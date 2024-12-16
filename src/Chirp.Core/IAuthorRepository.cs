namespace Chirp.Core;

public interface IAuthorRepository
{
    public List<Author> TEMPgetAUTHORwithEMAIL(string email);
    public List<Author> GetAuthor(string username);
    public Author AddAuthor(string name, string email);
    public List<Author> GetAuthorUserName(string userName);
    public List<AuthorDTO> GetFollowers(string email);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public bool IsFollowing(string email, string followingUsername);
    public void AddFollow(string user, string following);
    public void UnFollow(string user, string unfollowing);
    public bool IsBlocked(string userEmail, string blockEmail);
    public void CreateBlock(string userEmail, string blockemail);
    public void ForgetUser(string email);
    public bool IsFollowingUserName(string username, string author);
    public int GetFollowerCount(string email);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);

}