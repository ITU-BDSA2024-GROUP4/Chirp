using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Web.Pages.Partials;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    [BindProperty]
    public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty]
    public string Author { get; set; }
    [BindProperty]
    public string Author_Email { get; set; }
    
    public string Email { get; set; }
    public string UserEmail { get; set; }
    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet(string author)
    {
        SetCheeps();
        return Page();
    }
    public void SetCheeps() {
        string author = "";
        string url = HttpContext.Request.GetDisplayUrl();
        var match = Regex.Match(url, @"(?<=^https?://[^/]+/)([^?]+)");
        if (match.Success)
        {
            Console.WriteLine("------");
            Console.WriteLine(match.Value);
            author = match.Value;
        } else {
            return;
        }
        // if (!ModelState.IsValid)
        // {
        //     return Page();
        // }

        var pageQuery = Request.Query["page"].ToString();
        Email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (pageQuery == null)
        {

            Cheeps = _service.GetCheepsFromAuthor(author, 0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);
            Cheeps = _service.GetCheepsFromAuthor(author, page-1);
        }
    }
    public IActionResult OnPost()
    {
        SetCheeps();   
        
        string author = _service.GetOrCreateAuthor(User.Identity.Name, Email).Idenitifer;
        _service.CreateCheep(author, SubmitMessage.Message);

        return RedirectToPage();
        
    }
    public IActionResult OnPostFollow()
    {
        UserEmail = HelperMethods.FindEmail(User);

        switch (HelperMethods.Follow(ModelState, _service, nameof(Author_Email), nameof(Author), UserEmail,
                    User.Identity.Name, Author_Email))
        {
            case "Error":
                return RedirectToPage("/Error");
            case "UserTimeline":
                return RedirectToPage("/UserTimeline", new { author = Author });
            default:
                return RedirectToPage("/Error");
        }
    }
    public IActionResult OnPostUnfollow()
    {
        SetCheeps();
        UserEmail = HelperMethods.FindEmail(User);

        switch (HelperMethods.Unfollow(ModelState, _service, nameof(Author_Email), nameof(Author), UserEmail,
                    Author_Email, SubmitMessage))
        {
            case "Error":
                return RedirectToPage("/Error");
            case "Page":
                return Page();
            default:
                return RedirectToPage("/Error");
        }
    }
    public bool BlackMagic(string Author_Email)
    {
        UserEmail = HelperMethods.FindEmail(User);

        try
        {
            string a0 = _service.GetAuthor(UserEmail).Idenitifer;
            string a1 = _service.GetAuthor(Author_Email).Idenitifer;
            return _service.IsFollowing(a0, a1).Boolean;
        }
        catch (Exception e)
        {
            return false;
        }
    }
}
