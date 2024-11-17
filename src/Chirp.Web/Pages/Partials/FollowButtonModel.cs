#nullable disable

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;

namespace Chirp.Web.Pages;
    
public class FollowButtonModel
{
    private ICheepService _service { get; set; }
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
        return HelperMethods.IsFollowing(_service, UserEmail, Author_Email);
    }
}