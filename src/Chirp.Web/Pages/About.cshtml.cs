

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;
using Chirp.Web.Pages.Utils;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Chirp.Web.Pages;

public class AboutModel : PageModel {
    private readonly ICheepService _service;
    public string Author;
    public string UserEmail;
    public bool UserIsAuthor;
    public List<AuthorDTO> Following;
    public List<CheepDTO> Cheeps;

    public AboutModel(ICheepService service)
    {
        _service = service;
    }
    public ActionResult OnGet(string author)
    {
        UserIsAuthor = author.Equals(UserHandler.FindName(User));
        Author = author;
        UserEmail = UserHandler.FindEmail(User);

        Following = GetFollowers();
        Cheeps = GetCheeps();

        return Page();
    }
    public string GetEmail() 
    {
        return _service.GetAuthor(UserEmail).Email;
    }
    public string GetName() 
    {
        return _service.GetAuthor(UserEmail).Name;
    }
    public List<AuthorDTO> GetFollowers()
    {
        return _service.GetFollowers(UserEmail);
    }
    public List<CheepDTO> GetCheeps()
    {
        return _service.GetCheepsFromAuthor(Author);
    }
}