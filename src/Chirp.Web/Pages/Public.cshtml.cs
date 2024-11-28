using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using Chirp.Core;


using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

using System.Security.Claims;

using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;
using Chirp.Web.Pages.Utils;

using Chirp.Infrastructure;

using Microsoft.AspNetCore.Identity;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
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
    [BindProperty]
    public int Cheep_Id { get; set; }
    // Needs to be changed to use bindproperty, feels unnessecary to use in this case
    // [BindProperty]
    public FollowButtonModel FollowButton { get; set; }
    public bool InvalidCheep { get; set; } = false;

    public string UserEmail { get; set; }

    public PublicModel(ICheepService service, SignInManager<ChirpUser> signInManager)
    {
        _service = service;
        _signInManager = signInManager;
    }

    public void SetEmail() {
        UserEmail = UserHandler.FindEmail(User);
    }

    public ActionResult OnGet()
    {
        SetCheeps();
        return Page();
    }

    public void SetCheeps()
    {

        SetEmail();
        var pageQuery = Request.Query["page"].ToString();

        if (pageQuery == null)
        {
            Cheeps = _service.GetCheeps(0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);
            Cheeps = _service.GetCheeps(page - 1); // minus 1 because pages are 0 indexed   
        }

        FollowButton = new FollowButtonModel(_service, Cheeps, UserEmail);
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
        if(InvalidCheep) {
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
                return Page();
            default:
                return RedirectToPage("/Error");
        }
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
}