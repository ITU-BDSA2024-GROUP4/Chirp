using System.ComponentModel.DataAnnotations;

using Chirp.Core;

namespace Chirp.Infrastructure;


public class AuthorRepository : IAuthorRepository
{
    private readonly ChirpDBContext _context;

    public AuthorRepository(ChirpDBContext context)
    {
        _context = context;
    }

    // Command
    public Author AddAuthor(string username, string email)
    {
        Author author = new Author()
        {
            AuthorId = _context.Authors.Count() + 1,
            Name = username,
            Email = email,
            Cheeps = new List<Cheep>()
        };

        _context.Authors.Add(author);

        _context.SaveChanges();

        return author;
    }
    // Query
    public List<Author> GetAuthor(string username)
    {
        var query = (from Author in _context.Authors
                     where Author.Name == username
                     select Author);

        return query.ToList();
    }

    // Query
    public List<AuthorDTO> GetFollowers(string username)
    {
        var query = (from Follows in _context.Following
                     where Follows.User.Name == username
                     select new AuthorDTO { Name = Follows.Following.Name, Email = Follows.Following.Email, });
        return query.ToList();
    }

    // Query
    // TODO: why input?
    public List<AuthorDTO> GetBlockedAuthors(string username)
    {
        var query = (from Author in _context.Authors
                     join Blocked in _context.Blocked on Author.Name equals Blocked.User.Name
                     select new AuthorDTO
                     {
                         Name = Blocked.BlockedUser.Name,
                         Email = Blocked.BlockedUser.Email,
                     });
        return query.ToList();
    }

    // Query
    public bool IsFollowing(string authorUsername, string authorFollowingUsername)
    {

        var query = (from Follows in _context.Following
                     where Follows.User.Name == authorUsername && Follows.Following.Name == authorFollowingUsername
                     select Follows).Any();

        return query;
    }
    //TODO: Multiple commands
    public void AddFollow(string username, string followingUsername)
    {
        Author AuthorUser = GetAuthor(username)[0];
        Author AuthorFollowing = GetAuthor(followingUsername)[0];

        bool alreadyFollowing = _context.Following.Any(f =>
            f.User.AuthorId == AuthorUser.AuthorId &&
            f.Following.AuthorId == AuthorFollowing.AuthorId);

        if (alreadyFollowing)
        {
            throw new ApplicationException("TooManyFollows");
        }

        Follows follows = new Follows() { User = AuthorUser, Following = AuthorFollowing };

        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(follows);
        if (!Validator.TryValidateObject(follows, valContext, validationResults, true))
        {
            throw new ValidationException("Cheep validation failed: " + string.Join(", ", validationResults));
        }

        _context.Following.Add(follows);
        _context.SaveChanges();
    }
    // Command
    public void RemoveFollow(string username, string unfollowingUsername)
    {
        foreach (var follow in GetPersonToUnfollow(username, unfollowingUsername))
        {
            _context.Following.Remove(follow);
        }
        _context.SaveChanges();
    }

    //TODO: Multiple Commands and also query
    public List<Follows> GetPersonToUnfollow(string username, string unfollowingUsername)
    {
        var query = (from Follows in _context.Following
                     where Follows.User.Name == username && Follows.Following.Name == unfollowingUsername
                     select Follows);
        return query.ToList();
    }
    // Command
    public void CreateBlock(Author user, Author blockUser)
    {
        Blocked blocked = new Blocked() { User = user, BlockedUser = blockUser };

        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(blocked);
        if (!Validator.TryValidateObject(blocked, valContext, validationResults, true))
        {
            throw new ValidationException("Block validation failed: " + string.Join(", ", validationResults));
        }

        _context.Blocked.Add(blocked);
        _context.SaveChanges();
    }

    // Query
    public int IsBlocked(string username, string blockUsername)
    {
        var query = (from Blocked in _context.Blocked
                     where Blocked.User.Name == username && Blocked.BlockedUser.Name == blockUsername
                     select Blocked).Count();
        return query;
    }

    // Command(s)
    public void ForgetUser(Author user)
    {
        var cheeps = _context.Cheeps.Where(c => c.AuthorId == user.AuthorId).ToList();
        _context.Cheeps.RemoveRange(cheeps);

        var follows = _context.Following.Where(f => f.User.AuthorId == user.AuthorId || f.Following.AuthorId == user.AuthorId).ToList();
        _context.Following.RemoveRange(follows);

        _context.Authors.Remove(user);

        _context.SaveChanges();
    }

    // Query
    public int GetFollowerCount(string username)
    {
        var query = (from Follows in _context.Following
                     where Follows.Following.Name == username
                     select Follows).Count();
        return query;

    }

    // Query
    public int GetFollowerCountUserName(string username)
    {
        var query = (from Follows in _context.Following
                     where Follows.Following.Name == username
                     select Follows).Count();
        return query;

    }

    // Query
    public int GetFollowingCount(string username)
    {
        var query = (from Follows in _context.Following
                     where Follows.User.Name == username
                     select Follows).Count();
        return query;

    }

    // Query
    public bool IsFollowingUserName(string username, string followingUsername)
    {
        var query = (from Follows in _context.Following
                     where Follows.User.Name == username && Follows.Following.Name == followingUsername
                     select Follows).Any();

        return query;
    }
}