﻿using System.ComponentModel.DataAnnotations;

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
    public List<Author> GetAuthor(string email) {
        var query = (from Author in _context.Authors
            where Author.Email == email
            select Author);

        return query.ToList();
    }
    
    // Command
    public List<Author> GetAuthorUserName(string userName)
    {
        var query = (from Author in _context.Authors
            where Author.Name == userName
            select Author);

        return query.ToList();
    }
    
    // Query
    public List<AuthorDTO> GetFollowers(string email)
    {
        var query = (from Follows in _context.Following
            where Follows.User.Email == email
            select new AuthorDTO { Name = Follows.Following.Name, Email = Follows.Following.Email, });
        return query.ToList();
    }
    
    // Query
    public List<AuthorDTO> GetBlockedAuthors(string userEmail)
    {
        var query = (from Author in _context.Authors
            join Blocked in _context.Blocked on Author.Email equals Blocked.User.Email
            select new AuthorDTO
            {
                Name = Blocked.BlockedUser.Name, Email = Blocked.BlockedUser.Email,
            });
        return query.ToList();
    }
    
    // TODO: CHANGE IS BOTH COMMANDS AND QUERY
    public bool IsFollowing(string email, string authorEmail)
    {
        List<Author> AuthorUserList = GetAuthor(email);
        List<Author> AuthorAuthorList = GetAuthor(authorEmail);
        if (AuthorUserList.Count != 1 || AuthorAuthorList.Count != 1) {
            return false;
        }
        Author AuthorUser = AuthorUserList[0];
        Author AuthorAuthor = AuthorAuthorList[0];

        var query = (from Follows in _context.Following
            where Follows.User.AuthorId == AuthorUser.AuthorId && Follows.Following.AuthorId == AuthorAuthor.AuthorId
            select Follows).Any();

        return query;
    }
    //TODO: Multiple commands
    public void AddFollow(string user, string following)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Author AuthorFollowing = GetAuthor(following)[0];
        
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
    //TODO: rename remove follow
    public void UnFollow(string user, string unfollowing)
    {
        foreach (var follow in GetPersonToUnfollow(user, unfollowing))
        {
            _context.Following.Remove(follow);
        }
        _context.SaveChanges();
    }

    //TODO: Multiple Commands and also query
    public List<Follows> GetPersonToUnfollow(string user, string unfollowing)
    {
        Author AuthorUser = GetAuthor(user)[0];
        Author AuthorUnfollowing = GetAuthor(unfollowing)[0];
        var query = (from Follows in _context.Following
            where Follows.User.AuthorId == AuthorUser.AuthorId && Follows.Following.AuthorId == AuthorUnfollowing.AuthorId
            select Follows);
        return query.ToList();
    }
    //TODO: Multiple Commands
    public void CreateBlock(string userEmail, string blockEmail)
    {
        Author AuthorUser = GetAuthor(userEmail)[0];
        Author AuthorBlocking = GetAuthor(blockEmail)[0];
        
        Blocked blocked = new Blocked() { User = AuthorUser, BlockedUser = AuthorBlocking };
        
        var validationResults = new List<ValidationResult>();
        var valContext = new ValidationContext(blocked);
        if (!Validator.TryValidateObject(blocked, valContext, validationResults, true))
        {
            throw new ValidationException("Block validation failed: " + string.Join(", ", validationResults));
        }
        
        _context.Blocked.Add(blocked);
        _context.SaveChanges();
    }
    
    // Query TODO: boolean logic moved to service
    public bool IsBlocked(string userEmail, string blockEmail)
    {
        var query = (from Blocked in _context.Blocked
            where Blocked.User.Email == userEmail && Blocked.BlockedUser.Email == blockEmail
            select Blocked).Count();
        return query > 0;
    }
    
    // TODO: Both query and command
    public void ForgetUser(string email)
    {
        var author = _context.Authors.SingleOrDefault(a => a.Email.ToLower() == email.ToLower());        if (author == null)
        {
            throw new Exception("User not found.");
        }

        
        var cheeps = _context.Cheeps.Where(c => c.AuthorId == author.AuthorId).ToList();
        _context.Cheeps.RemoveRange(cheeps);

        
        var follows = _context.Following.Where(f => f.User.AuthorId == author.AuthorId || f.Following.AuthorId == author.AuthorId).ToList();
        _context.Following.RemoveRange(follows);

        
        _context.Authors.Remove(author);

        
        _context.SaveChanges();
    }
    
    // Query
    public int GetFollowerCount(string email)
    {
        var query = (from Follows in _context.Following
            where Follows.Following.Email == email
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
    
    // TODO: Multiple queries, move logic to service
    public bool IsFollowingUserName(string username, string author)
    {
        List<Author> AuthorUserList = GetAuthorUserName(username);
        List<Author> AuthorAuthorList = GetAuthor(author);
        if (AuthorUserList.Count != 1 || AuthorAuthorList.Count != 1) {
            return false;
        }
        Author AuthorUser = AuthorUserList[0];
        Author AuthorAuthor = AuthorAuthorList[0];

        var query = (from Follows in _context.Following
            where Follows.User.AuthorId == AuthorUser.AuthorId && Follows.Following.AuthorId == AuthorAuthor.AuthorId
            select Follows).Any();

        return query;
    }
}