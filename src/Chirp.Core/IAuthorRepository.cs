namespace Chirp.Core;

public interface IAuthorRepository
{
    public List<Author> TEMPgetAUTHORwithEMAIL(string email);
    public List<Author> GetAuthor(string username);
    public Author AddAuthor(string name, string email);
    public List<Author> GetAuthorUserName(string userName);
    public List<AuthorDTO> GetFollowers(string email);
    public List<AuthorDTO> GetBlockedAuthors(string userEmail);
    public bool IsFollowing(string username, string followingUsername);
    public void AddFollow(string username, string followingUsername);
    public void UnFollow(string username, string unfollowingUsername);
    public bool IsBlocked(string username, string blockUsername);
    public void CreateBlock(string username, string blockUsername);
    public void ForgetUser(string email);
    public bool IsFollowingUserName(string username, string followingUsername);
    public int GetFollowerCount(string email);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);
    public List<Follows> GetPersonToUnfollow(string username, string unfollowingUsername);

}