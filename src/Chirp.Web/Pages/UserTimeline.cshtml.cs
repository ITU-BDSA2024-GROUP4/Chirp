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
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly SignInManager<ChirpUser> _signInManager;

    public List<CheepDTO> Cheeps { get; set; } = null!;
    [BindProperty] public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty(SupportsGet = true)] public string Author { get; set; }

    [BindProperty] public string Author_Email { get; set; }

    // Needs to be changed to use bindproperty, feels unnessecary to use in this case
    // [BindProperty]
    [BindProperty] public int Cheep_Id { get; set; }
    public FollowButtonModel FollowButton { get; set; }
    public bool InvalidCheep { get; set; } = false;

    public string Email { get; set; }
    public string TEMPUserEmail { get; set; }
    public string Username { get; set; }
    public int CurrentPage { get; set; } = 0;

    public UserTimelineModel(ICheepService cheepService, IAuthorService authorService, SignInManager<ChirpUser> signInManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _signInManager = signInManager;
    }

    public void TEMPSetEmail()
    {
        TEMPUserEmail = UserHandler.FindEmail(User);
    }
    public void SetUsername()
    {
        Username = UserHandler.FindName(User);
    }

    public ActionResult OnGet(string author)
    {
        SetCheeps();
        return Page();
    }

    public void SetCheeps()
    {
        TEMPSetEmail();
        SetUsername();

        var pageQuery = Request.Query["page"].ToString();
        Email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (pageQuery != null)
        {
            _ = int.TryParse(pageQuery, out int page);
            CurrentPage = page;
        }

        if (Author == User.Identity.Name)
        {
            Cheeps = _cheepService.GetOwnTimelinePage(TEMPUserEmail, CurrentPage);
        }
        else
        {
            Cheeps = _cheepService.GetCheepsFromAuthorPage(Author, CurrentPage);
        }

        FollowButton = new FollowButtonModel(_cheepService, _authorService, Cheeps, TEMPUserEmail, Username, Author == User.Identity.Name); 
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

        if (!InvalidCheep)
        {
            InvalidCheep = false;
        }
        string username = _authorService.GetOrCreateAuthor(User.Identity.Name, TEMPUserEmail).Name;
        _cheepService.AddCheep(username, SubmitMessage.Message);

        SubmitMessage.Message = ""; //Clears text field
        return RedirectToPage();
    }

    public IActionResult OnPostFollow()
    {
        TEMPSetEmail();
        SetUsername();

        switch (FollowHandler.Follow(ModelState, _authorService, nameof(Author_Email), nameof(Author), TEMPUserEmail,
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

        switch (FollowHandler.Unfollow(ModelState, _authorService, nameof(Author_Email), nameof(Author), TEMPUserEmail,
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
        _cheepService.CreateLike(TEMPUserEmail, Cheep_Id);
        return RedirectToPage();
    }

    public IActionResult OnPostUnlike()
    {
        SetCheeps();
        _cheepService.UnLike(Username, Cheep_Id);
        return RedirectToPage();
    }

    public bool GetMaxPage()
    {
        if (Author == User.Identity.Name)
        {
            return CurrentPage <= (_cheepService.GetOwnTimeline(TEMPUserEmail).Count() / 32);
        }
        else
        {
            return CurrentPage <= (_cheepService.GetCheepsFromAuthor(Author).Count() / 32); //32 is got from repository "_pagesize"
        }
    }

    public int Followers()
    {
        return _authorService.GetFollowerCountUserName(Author);
    }

    public string TEMPGetEmail()
    {
        return _authorService.GetAuthorUserName(Author).Email;
    }

    public string GetUsername()
    {
        return _authorService.GetAuthorUserName(Author).Name;
    }

    public bool IsFollowing()
    {
        return _authorService.IsFollowing(TEMPUserEmail, TEMPGetEmail());
    }

    public int GetFollowingCount()
    {
        return _authorService.GetFollowingCount(Author);
    }

    public int GetTotalLikesCount()
    {
        return _cheepService.TotalLikeCountUser(GetUsername());
    }

    public int GetTotalCheepsCount()
    {
        return _cheepService.GetTotalCheeps(Author);
    }

    public IActionResult OnPostBlock()
    {
        SetCheeps();
        _authorService.CreateBlock(TEMPUserEmail, TEMPGetEmail());
        _cheepService.UserBlockedSomeone(TEMPUserEmail);
        return Redirect("~/");    
    }

    public IActionResult OnPostDeleteCheep()
    {
        SetCheeps();
        if (TEMPGetEmail() != Author_Email)
        {
            throw new Exception("Author Email is not the logged in user.");
        }
        _cheepService.RemoveCheep(Username, Cheep_Id);
        return RedirectToPage();
    }
}