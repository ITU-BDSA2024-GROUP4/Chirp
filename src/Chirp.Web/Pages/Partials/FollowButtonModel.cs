#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Web.Pages;
    
public class FollowButtonModel
{
    public ICheepService _service { get; set; }
    public List<CheepDTO> Cheeps { get; set; }
    public string UserEmail { get; set; }
    public bool ShowOnCheeps { get; set; }
    public bool isMyOwnCheep { get; set; }
    
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

    public bool IsMyOwnCheep(string Author_Email)
    {
        if (Author_Email == UserEmail)
        {
            return true;
        } 
        return false;
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