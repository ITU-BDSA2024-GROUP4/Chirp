namespace Chirp.Core;

public interface IAuthorRepository
{
    public List<Author> GetAuthor(string username);
    public Author AddAuthor(string name, string email);
    public List<AuthorDTO> GetFollowers(string username);
    public List<AuthorDTO> GetBlockedAuthors(string username);
    public bool IsFollowing(string authorUsername, string authorFollowingUsername);
    public void AddFollow(string username, string followingUsername);
    public void RemoveFollow(string username, string unfollowingUsername);
    public int IsBlocked(string username, string blockUsername);
    public void CreateBlock(Author user, Author blockUser);
    public void ForgetUser(Author username);
    public bool IsFollowingUserName(string username, string followingUsername);
    public int GetFollowerCount(string username);
    public int GetFollowerCountUserName(string username);
    public int GetFollowingCount(string username);
    public List<Follows> GetPersonToUnfollow(string username, string unfollowingUsername);

}