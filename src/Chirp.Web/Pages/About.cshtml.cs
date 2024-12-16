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

public class AboutModel : PageModel {
    private readonly ICheepService _cheepService;
    private readonly IAuthorService _authorService;
    private readonly SignInManager<ChirpUser> _signInManager;
    private readonly UserManager<ChirpUser> _userManager;
    public string Username;
    public bool UserIsAuthor;
    public List<AuthorDTO> Following;
    public List<CheepDTO> Cheeps;
    public List<CheepDTO> Likes;
    [BindProperty] public string Unblock_Username { get; set; }

    public AboutModel(ICheepService cheepService, IAuthorService authorService,SignInManager<ChirpUser> signInManager, UserManager<ChirpUser> userManager)
    {
        _cheepService = cheepService;
        _authorService = authorService;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public ActionResult OnGet(string author)
    {
        UserIsAuthor = author.Equals(UserHandler.FindName(User));
        SetInformation();
        return Page();
    }

    public void SetInformation()
    {
        Username = UserHandler.FindName(User);
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
        return _authorService.GetAuthor(Username).Email;
    }

    public string GetName()
    {
        return _authorService.GetAuthor(Username).Name;
    }

    public List<AuthorDTO> GetFollowers()
    {
        return _cheepService.GetFollowers(Username);
    }

    public List<CheepDTO> GetCheeps()
    {
        AuthorDTO authorDTO = _authorService.GetAuthor(Username);
        if (authorDTO == null)
        {
            return new List<CheepDTO>();
        }
        return _cheepService.GetCheepsFromAuthor(authorDTO.Name);
    }

    public List<CheepDTO> GetLikes()
    {
        return _cheepService.GetLiked(Username);
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
        _authorService.ForgetMe(Username);
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
        return _cheepService.UserBlockedSomeone(Username);
    }

    public IActionResult OnPostUnblock()
    {
        SetInformation();
        
        _cheepService.UnBlock(Username, Unblock_Username);
        
        return RedirectToPage();
    }

    public List<AuthorDTO> GetBlockedAuthors()
    {
        return _authorService.GetBlockedAuthors(Username);
    }
}