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
    [BindProperty] 
    public SubmitMessageModel SubmitMessage { get; set; }
    [BindProperty] 
    public string Author { get; set; }
    [BindProperty] 
    public string Author_Email { get; set; }
    public bool InvalidCheep { get; set; } = false;

    public string UserEmail { get; set; }

    public PublicModel(ICheepService service)
    {
        _service = service;
    }
    public void SetEmail() {
        UserEmail = HelperMethods.FindEmail(User);
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
        Console.WriteLine("OnPostFollow Called");
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
        Console.WriteLine("OnPostFollow Called");
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

    public bool IsFollowing(string Author_Email)
    {
        SetEmail();
        return HelperMethods.IsFollowing(_service, UserEmail, Author_Email);
    }
}