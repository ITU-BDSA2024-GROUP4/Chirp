namespace Chirp.Core;

public interface IAuthorService
{
    public void AddAuthor(string name, string email);
    public List<AuthorDTO> GetBlockedAuthors(string username);
    public AuthorDTO GetAuthor(string username);
    public AuthorDTO GetAuthorUserName(string username);
    public AuthorDTO GetOrCreateAuthor(string username, string email);
    public bool IsFollowing(string username, string followingUsername);
    public void CreateFollow(string username, string user, string followingUsername);
    public void CreateBlock(string userEmail, string blockemail);
    public bool IsBlocked(string username, string blockUsername);
    public void UnFollow(string username, string unfollowUsername);
    public void ForgetMe(string username);
    public int GetFollowerCount(string username);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);
    public bool IsFollowingUserName(string username, string followingUsername);
    public List<AuthorDTO> GetFollowers(string username);
}