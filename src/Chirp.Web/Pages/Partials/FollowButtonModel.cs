#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;

namespace Chirp.Web.Pages;
    
public class FollowButtonModel
{
    public ICheepService _service { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public string UserEmail { get; set; }
    public bool ShowOnCheeps { get; set; }
    
    public FollowButtonModel(ICheepService service, List<CheepDTO> cheeps, string userEmail, bool showOnCheeps = true) 
    {
        _service = service;
        Cheeps = cheeps;
        UserEmail = userEmail;
        ShowOnCheeps = showOnCheeps;
    }

    public bool IsFollowing(string Author_Email)
    {
        return _service.IsFollowing(UserEmail, Author_Email);
    }

    public bool IsLiked(string user, int Cheep_Id)
    {
        return _service.IsLiked(user, Cheep_Id);
    }

    public int LikeCount(int Cheep_Id)
    {
        return _service.LikeCount(Cheep_Id);
    }

    public int FollowCount(string email)
    {
        return _service.GetFollowerCount(email);
    }
}