

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
    public bool UserIsAuthor;

    public AboutModel(ICheepService service)
    {
        _service = service;
    }
    public ActionResult OnGet(string author)
    {
        UserIsAuthor = author.Equals(UserHandler.FindName(User));
        Author = author;
        return Page();
    }
}