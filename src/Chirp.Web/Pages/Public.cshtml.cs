using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    [BindProperty]
    public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty]
    public string Author { get; set; }
    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        SetCheeps();
        return Page();
    }
    public void SetCheeps() {
        var pageQuery = Request.Query["page"].ToString();
        
        if (pageQuery == null)
        {
            Cheeps = _service.GetCheeps(0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);            
            Cheeps = _service.GetCheeps(page-1); // minus 1 because pages are 0 indexed   
        }
    }

    //code credit to Adrian <adrianjuul123@gmail.com>
    public IActionResult OnGetLogin()
    {
        return Challenge(new AuthenticationProperties{
            RedirectUri = "/"
        },"GitHub");
    }

    //code credit to Adrian <adrianjuul123@gmail.com>
    public IActionResult OnGetLogout()
    {
        return SignOut(new AuthenticationProperties{
            RedirectUri = "/"
        }, CookieAuthenticationDefaults.AuthenticationScheme);
    }
    public IActionResult OnPost()
    {
        SetCheeps();
        if (HelperMethods.IsInvalid(nameof(SubmitMessage.Message), ModelState))
        {
            return Page();
        }
        string userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

        string author = _service.GetOrCreateAuthor(User.Identity.Name, userEmail).Idenitifer;
        _service.CreateCheep(author, SubmitMessage.Message);

        return RedirectToPage();
    }
    public IActionResult OnPostFollow()
    {
        if (HelperMethods.IsInvalid(nameof(Author), ModelState))
        {
            return RedirectToPage("/Stoooooooooooooopit");
        }
        return RedirectToPage("/UserTimeline", new { author = Author });
    }
}