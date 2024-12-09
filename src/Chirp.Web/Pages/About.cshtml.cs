#nullable disable
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using System.IO.Compression;
using System.Text;

using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Utils;

using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Elfie.Extensions;

namespace Chirp.Web.Pages;

public class AboutModel : PageModel
{
    private readonly ICheepService _service;
    private readonly SignInManager<ChirpUser> _signInManager;
    private readonly UserManager<ChirpUser> _userManager;
    public string Author;
    public string UserEmail;
    public bool UserIsAuthor;
    public List<AuthorDTO> Following;
    public List<CheepDTO> Cheeps;
    public List<CheepDTO> Likes;
    [BindProperty] public string User_Email { get; set; }
    [BindProperty] public string Unblock_User { get; set; }

    public AboutModel(ICheepService service, SignInManager<ChirpUser> signInManager, UserManager<ChirpUser> userManager)
    {
        _service = service;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public ActionResult OnGet(string author)
    {
        UserIsAuthor = author.Equals(UserHandler.FindName(User));
        Author = author;
        SetInformation();
        return Page();
    }

    public void SetInformation()
    {
        UserEmail = UserHandler.FindEmail(User);
        Following = GetFollowers();
        Cheeps = GetCheeps();
        Likes = GetLikes();
    }

    public ActionResult OnPostDownload()
    {
        SetInformation(); // Ensure data is up-to-date

        var csvContent = CreateCsvContent();

        var memoryStream = new MemoryStream();
        try
        {
            using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                var entry = zipArchive.CreateEntry("Userdata.csv");
                using (var entryStream = entry.Open())
                using (var writer = new StreamWriter(entryStream, Encoding.UTF8))
                {
                    writer.Write(csvContent);
                }
            }

            memoryStream.Position = 0;

            return File(memoryStream, "application/zip", $"Userdata-{DateTime.Now:yyyy-MMM-dd-HHmmss}.zip");
        }
        catch
        {
            memoryStream.Dispose();
            throw;
        }
    }

    public string GetEmail()
    {
        return _service.GetAuthor(UserEmail).Email;
    }

    public string GetName()
    {
        return _service.GetAuthor(UserEmail).Name;
    }

    public List<AuthorDTO> GetFollowers()
    {
        return _service.GetFollowers(UserEmail);
    }

    public List<CheepDTO> GetCheeps()
    {
        AuthorDTO authorDTO = _service.GetAuthor(UserEmail);
        if (authorDTO == null)
        {
            return new List<CheepDTO>();
        }

        return _service.GetCheepsFromAuthor(authorDTO.Name);
    }

    public List<CheepDTO> GetLikes()
    {
        return _service.GetLiked(UserEmail);
    }

    public string CreateCsvContent()
    {
        var sb = new StringBuilder();

        sb.AppendLine("Category,Data");
        sb.AppendLine($"Email,{GetEmail()}");
        sb.AppendLine($"Name,{GetName()}");

        sb.AppendLine("Following");
        foreach (var author in Following)
        {
            sb.AppendLine($",{author.Name}");
        }

        sb.AppendLine("Cheeps");
        foreach (var cheep in Cheeps)
        {
            sb.AppendLine($",{cheep.Message}");
        }

        sb.AppendLine("Cheeps you liked");
        foreach (var cheep in Likes)
        {
            sb.AppendLine($"{cheep.Author},{cheep.Message}");
        }

        return sb.ToString();
    }

    public async Task<IActionResult> OnGetLogout()
    {
        var signOut = await Authentication.HandleLogout(_signInManager, this);
        return signOut;
    }

    public async Task<IActionResult> OnPostForgetMe()
    {
        UserEmail = UserHandler.FindEmail(User);
        _service.ForgetMe(UserEmail);
        var userId = _userManager.GetUserId(User);
        var chirpUser = await _userManager.FindByIdAsync(userId);
        if (chirpUser == null)
        {
            throw new Exception();
        }

        var result = await _userManager.DeleteAsync(chirpUser);
        return await Authentication.HandleLogout(_signInManager, this);
    }

    public bool UserBlockedSomeOne()
    {
        return _service.UserBlockedSomeone(UserEmail);
    }

    public IActionResult OnPostUnblock()
    {
        User_Email = UserHandler.FindEmail(User);

        _service.UnBlock(User_Email, Unblock_User);


        return RedirectToPage();
    }

    public List<AuthorDTO> GetBlockedAuthors()
    {
        return _service.GetBlockedAuthors(UserEmail);
    }
}