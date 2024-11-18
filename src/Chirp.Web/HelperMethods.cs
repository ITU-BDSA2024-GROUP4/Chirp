using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Chirp.Core;
using Chirp.Infrastructure;
using Chirp.Web.Pages.Partials;

namespace Chirp.Web;
public static class HelperMethods {

    public static string FindEmail(ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }
    
}