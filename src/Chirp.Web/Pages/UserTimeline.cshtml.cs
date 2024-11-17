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
    // Needs to be changed to use bindproperty, feels unnessecary to use in this case
    // [BindProperty]
    public FollowButtonModel FollowButton { get; set; }
    public bool InvalidCheep { get; set; } = false;
    
    public string Email { get; set; }
    public string UserEmail { get; set; }
    public UserTimelineModel(ICheepService service)
    {
        _service = service;
    }
    public void SetEmail() {
        UserEmail = HelperMethods.FindEmail(User);
    }
    public ActionResult OnGet(string author)
    {
        SetCheeps();
        return Page();
    }
    public void SetCheeps() {
        SetEmail();
        string url = HttpContext.Request.GetDisplayUrl();
        //Uses a regex to find the user in the url
        var match = Regex.Match(url, @"(?<=^https?://[^/]+/)([^?]+)");
        if (!match.Success)
        {
            throw new Exception("Url not matching");
        }

        var pageQuery = Request.Query["page"].ToString();
        Email = User.FindFirst(ClaimTypes.Email)?.Value;

        if (pageQuery == null)
        {

            Cheeps = _service.GetCheepsFromAuthor(match.Value, 0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);
            Cheeps = _service.GetCheepsFromAuthor(match.Value, page-1);
        }

        FollowButton = new FollowButtonModel(_service, Cheeps, UserEmail);
    }
    
    public IActionResult OnPost() 
    {
        //This is a fall back if there is no OnPost[HandlerName]
        SetCheeps();
        return Page();
    }

    public IActionResult OnPostMessage()
    {
        SetCheeps();
        if (HelperMethods.IsInvalid(nameof(SubmitMessage.Message), ModelState))
        {
            InvalidCheep = true;
            return Page();
        }
        if(!InvalidCheep) {
            InvalidCheep = false;
        }
        string author = _service.GetOrCreateAuthor(User.Identity.Name, UserEmail).Idenitifer;
        _service.CreateCheep(author, SubmitMessage.Message);

        SubmitMessage.Message = ""; //Clears text field
        return RedirectToPage(); 
    }

    public IActionResult OnPostFollow()
    {
        SetEmail();

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
}
