using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages;
    
public class SubmitMessageModel : PageModel
{
    [BindProperty] public string Message { get; set; } = null!;
}