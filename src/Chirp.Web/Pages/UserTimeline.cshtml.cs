#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.RegularExpressions;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;
using Chirp.Web.Pages.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Web.Pages;

public class UserTimelineModel : PageModel
{
    private readonly ICheepService _service;
    private readonly SignInManager<ChirpUser> _signInManager;

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
    public UserTimelineModel(ICheepService service, SignInManager<ChirpUser> signInManager)
    {
        _service = service;
        _signInManager = signInManager;
    }
    public void SetEmail() {
        UserEmail = UserHandler.FindEmail(User);
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
        int pageNumber = 0; //Defaults to first page
        if (!match.Success)
        {
            throw new Exception("Url not matching");
        }

        var pageQuery = Request.Query["page"].ToString();
        Email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (pageQuery != null)
        {
            _ = int.TryParse(pageQuery, out int page);
            pageNumber = page - 1;
        }
        Console.Write("USR IDENTITY: {0}", User.Identity.Name);
        Console.Write("MATCH VAL: {0}", match.Value);

        if (match.Value == User.Identity.Name)
        {
            Cheeps = _service.GetOwnTimeline(UserEmail, pageNumber);
        }
        else
        {
            Cheeps = _service.GetCheepsFromAuthorPage(match.Value, pageNumber); // default to first page
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
        if (StateValidator.IsInvalid(nameof(SubmitMessage.Message), ModelState))
        {
            InvalidCheep = true;
            return Page();
        }
        if(!InvalidCheep) {
            InvalidCheep = false;
        }
        string author = _service.GetOrCreateAuthor(User.Identity.Name, UserEmail).Email;
        _service.CreateCheep(author, SubmitMessage.Message);

        SubmitMessage.Message = ""; //Clears text field
        return RedirectToPage(); 
    }

    public IActionResult OnPostFollow()
    {
        SetEmail();

        switch (FollowHandler.Follow(ModelState, _service, nameof(Author_Email), nameof(Author), UserEmail,
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

        switch (FollowHandler.Unfollow(ModelState, _service, nameof(Author_Email), nameof(Author), UserEmail,
                    Author_Email, SubmitMessage))
        {
            case "Error":
                return RedirectToPage("/Error");
            case "Page":
                return RedirectToPage();
            default:
                return RedirectToPage("/Error");
        }
    }
    
    public IActionResult OnGetLogin()
    {
        return Authentication.HandleLogin(this);
    }

    public async Task<IActionResult> OnGetLogout()
    {
        var signOut = await Authentication.HandleLogout(_signInManager, this);
        return signOut;
    }
    
}
