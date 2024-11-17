using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core;
using Chirp.Web.Pages.Partials;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Chirp.Web.Pages;

public class PublicModel : PageModel
{
    private readonly ICheepService _service;
    public List<CheepDTO> Cheeps { get; set; } = null!;
    [BindProperty] public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty] public string Author { get; set; }
    [BindProperty] public string Author_Email { get; set; }

    public string UserEmail { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }

    public ActionResult OnGet()
    {
        SetCheeps();
        return Page();
    }

    public void SetCheeps()
    {
        var pageQuery = Request.Query["page"].ToString();
        UserEmail = HelperMethods.FindEmail(User);

        if (pageQuery == null)
        {
            Cheeps = _service.GetCheeps(0); // default to first page
        }
        else
        {
            _ = int.TryParse(pageQuery, out int page);
            Cheeps = _service.GetCheeps(page - 1); // minus 1 because pages are 0 indexed   
        }
    }

    //code credit to Adrian <adrianjuul123@gmail.com>
    public IActionResult OnGetLogin()
    {
        return Challenge(new AuthenticationProperties { RedirectUri = "/" }, "GitHub");
    }

    //code credit to Adrian <adrianjuul123@gmail.com>
    public IActionResult OnGetLogout()
    {
        return SignOut(new AuthenticationProperties { RedirectUri = "/" },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    public IActionResult OnPost()
    {
        SetCheeps();
        if (HelperMethods.IsInvalid(nameof(SubmitMessage.Message), ModelState))
        {
            return Page();
        }

        string author = _service.GetOrCreateAuthor(User.Identity.Name, UserEmail).Idenitifer;
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