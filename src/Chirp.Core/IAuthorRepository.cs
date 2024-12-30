namespace Chirp.Core
{
    public interface IAuthorRepository
    {
        // Author Management
        List<Author> GetAuthor(string username);
        Author AddAuthor(string name, string email);
        void ForgetUser(Author username);

        // Follower Management
        List<AuthorDTO> GetFollowers(string username);
        int GetFollowerCount(string username);
        int GetFollowerCountUserName(string username);
        bool IsFollowing(string authorUsername, string authorFollowingUsername);
        bool IsFollowingUserName(string username, string followingUsername);
        void AddFollow(string username, string followingUsername);
        void RemoveFollow(string username, string unfollowingUsername);

        // Following Management
        int GetFollowingCount(string username);
        List<Follows> GetPersonToUnfollow(string username, string unfollowingUsername);

        // Block Management
        List<AuthorDTO> GetBlockedAuthors(string username);
        int IsBlocked(string username, string blockUsername);
        void CreateBlock(Author user, Author blockUser);
    }
}