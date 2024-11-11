#nullable disable

using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages;
    
public class SubmitMessageModel
{
    [BindProperty]
    public string Message { get; set; }

}