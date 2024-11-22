using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Chirp.Web.Pages.Utils
{
    public static class Authentication
    {
        public static IActionResult HandleLogin(PageModel page, string redirectUri = "/")
        {
            return page.Challenge(
                new AuthenticationProperties { RedirectUri = redirectUri },
                "GitHub"
            );
        }

        public static IActionResult HandleLogout(PageModel page, string redirectUri = "/")
        {
            return page.SignOut(
                new AuthenticationProperties { RedirectUri = redirectUri },
                CookieAuthenticationDefaults.AuthenticationScheme
            );
        }
    }
}