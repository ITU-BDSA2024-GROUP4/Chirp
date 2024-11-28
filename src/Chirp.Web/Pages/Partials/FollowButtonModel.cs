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
    
    public FollowButtonModel(ICheepService service, List<CheepDTO> cheeps, string userEmail) 
    {
        _service = service;
        Cheeps = cheeps;
        UserEmail = userEmail;;
    }

    public bool IsFollowing(string Author_Email)
    {
        return _service.IsFollowing(UserEmail, Author_Email);
    }

    public bool IsLiked(string user, int Cheep_Id)
    {
        Console.Write("DEBUG LUCVIC " +  _service.IsLiked(user, Cheep_Id) );
        return _service.IsLiked(user, Cheep_Id);
    }
}