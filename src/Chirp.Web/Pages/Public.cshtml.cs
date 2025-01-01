#nullable disable
using AspNet.Security.OAuth.GitHub;

using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;
using Chirp.Web.Pages.Utils;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly SignInManager<ChirpUser> _signInManager;
    public List<CheepDTO> Cheeps { get; set; } = null!;

    [BindProperty] public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty] public string Author_Username { get; set; }

    [BindProperty] public int Cheep_Id { get; set; }

    // Needs to be changed to use bindproperty, feels unnessecary to use in this case
    // [BindProperty]
    public FollowButtonModel FollowButton { get; set; }
    public bool InvalidCheep { get; set; } = false;
    public string Username { get; set; }
    public int CurrentPage { get; set; }

    private readonly int _pagesize = 32;

    public PublicModel(ICheepService cheepService, IAuthorService authorService, SignInManager<ChirpUser> signInManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _signInManager = signInManager;
    }

    public void SetUserInfo()
    {
        Username = UserHandler.FindName(User);
    }

    public ActionResult OnGet()
    {
        SetCheeps();
        return Page();
    }

    public void SetCheeps()
    {
        SetUserInfo();
        var pageQuery = Request.Query["page"].ToString();
        if (!_cheepService.UserBlockedSomeone(Username))
        {
            if (pageQuery == null)
            {
                CurrentPage = 0;
                Cheeps = _cheepService.GetCheeps(0); // default to first page
            }
            else
            {
                _ = int.TryParse(pageQuery, out int page);
                CurrentPage = page;
                Cheeps = _cheepService.GetCheeps(page - 1); // minus 1 because pages are 0 indexed   
            }
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);
            CurrentPage = page;
            Cheeps = _cheepService.GetCheepsNotBlocked(Username); // minus 1 because pages are 0 indexed  
        }

        FollowButton = new FollowButtonModel(_cheepService, _authorService, Cheeps, Username);
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

    public IActionResult OnPost()
    {
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

        if (InvalidCheep)
        {
            InvalidCheep = false;
        }

        string username = _authorService.GetOrCreateAuthor(User.Identity.Name, UserHandler.FindEmail(User)).Name;
        _cheepService.AddCheep(username, SubmitMessage.Message);

        SubmitMessage.Message = ""; //Clears text field
        return RedirectToPage();
    }

    public IActionResult OnPostFollow()
    {
        SetUserInfo();

        switch (FollowHandler.Follow(ModelState, _authorService, nameof(Author_Username), Username,
                    User.Identity.Name, Author_Username))
        {
            case "Error":
                return RedirectToPage("/Error");
            case "UserTimeline":
                return RedirectToPage("/UserTimeline", new { author = Author_Username });
            default:
                return RedirectToPage("/Error");
        }
    }

    public IActionResult OnPostUnfollow()
    {
        SetCheeps();

        switch (FollowHandler.Unfollow(ModelState, _authorService, nameof(Author_Username), Username,
                    Author_Username, SubmitMessage))
        {
            case "Error":
                return RedirectToPage("/Error");
            case "Page":
                return RedirectToPage();
            default:
                return RedirectToPage("/Error");
        }
    }

    public IActionResult OnPostLike()
    {
        SetCheeps();
        _cheepService.CreateLike(Username, Cheep_Id);
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
        return CurrentPage <= (_cheepService.AmountOfCheeps() / _pagesize); //32 is got from repository "_pagesize"
    }

    public string GetLoggedUsername() { return _authorService.GetAuthorUserName(User.Identity.Name).Name; }
    public IActionResult OnPostDeleteCheep()
    {
        SetCheeps();
        if (GetLoggedUsername() != Author_Username)
        {
            throw new Exception("Author Email is not the logged in user.");
        }
        _cheepService.RemoveCheep(Username, Cheep_Id);
        return RedirectToPage();
    }
}