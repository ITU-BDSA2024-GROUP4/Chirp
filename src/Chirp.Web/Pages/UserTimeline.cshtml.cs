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
    [BindProperty(SupportsGet = true)]
    public string Author { get; set; }
    [BindProperty]
    public string Author_Email { get; set; }
    // Needs to be changed to use bindproperty, feels unnessecary to use in this case
    // [BindProperty]
    [BindProperty]
    public int Cheep_Id { get; set; }
    public FollowButtonModel FollowButton { get; set; }
    public bool InvalidCheep { get; set; } = false;
    
    public string Email { get; set; }
    public string UserEmail { get; set; }
    public int CurrentPage { get; set; } = 0;

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

        var pageQuery = Request.Query["page"].ToString();
        Email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (pageQuery != null)
        {
            _ = int.TryParse(pageQuery, out int page);
            CurrentPage = page;
        }

        if (Author == User.Identity.Name)
        {
            Cheeps = _service.GetOwnTimelinePage(UserEmail, CurrentPage);
        }
        else
        {
            Cheeps = _service.GetCheepsFromAuthorPage(Author, CurrentPage);
        }

        FollowButton = new FollowButtonModel(_service, Cheeps, UserEmail);
    }
    
    public IActionResult OnPost() 
    {
        //This is a fall back if there is no OnPost[HandlerName]
        SetCheeps();
        Console.WriteLine("Brah brhuh" + _service.GetFollowerCount(UserEmail));
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
    
    public IActionResult OnPostLike()
    {
        SetCheeps();
        _service.CreateLike(UserEmail, Cheep_Id);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike()
    {
        SetCheeps();
        _service.UnLike(UserEmail, Cheep_Id);
        return RedirectToPage();
    }
    public bool GetMaxPage()
    {
        if (Author == User.Identity.Name)
        {
            return CurrentPage <= (_service.GetOwnTimeline(UserEmail).Count() / 32);
        }
        else
        {
            return CurrentPage <= (_service.GetCheepsFromAuthor(Author).Count() / 32); //32 is got from repository "_pagesize"
        }
    }

    public int Followers()
    {
        return _service.GetFollowerCountUserName(Author);
    }
}
