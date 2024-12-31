#nullable disable

using System.ComponentModel.DataAnnotations;

using Chirp.Core;

using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class FollowButtonModel
{
    private ICheepService _cheepService { get; set; }
    private IAuthorService _authorService { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public string Username { get; set; }
    public bool ShowOnCheeps { get; set; }

    public FollowButtonModel(ICheepService cheepService, IAuthorService authorService, List<CheepDTO> cheeps, string username, bool showOnCheeps = true)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        Cheeps = cheeps;
        Username = username;
        ShowOnCheeps = showOnCheeps;
    }

    public bool IsFollowing(string username)
    {
        return _authorService.IsFollowing(Username, username); // TODO: change UserEmail to its username
    }

    public bool IsLiked(string username, int Cheep_Id)
    {
        return _cheepService.IsLiked(username, Cheep_Id);
    }

    public int LikeCount(int Cheep_Id)
    {
        return _cheepService.LikeCount(Cheep_Id);
    }

    public int FollowCount(string username)
    {
        return _authorService.GetFollowerCount(username);
    }
}