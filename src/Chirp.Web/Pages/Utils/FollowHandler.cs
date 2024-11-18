using Chirp.Core;
using Chirp.Web.Pages.Partials;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chirp.Web.Pages.Utils;

public static class FollowHandler
{
    public static bool IsInvalid(string input, ModelStateDictionary modelState)
    {
        foreach (var state in modelState)
        {
            if (state.Key.StartsWith(input))
            {
                foreach (var error in state.Value.Errors)
                {
                    return true; //Invalid because error :(
                }
                return false; //Its valid if exists and no error :)
            }
        }

        return true; //Invalid because not exist :(
    }
    public static string Follow(ModelStateDictionary modelState, ICheepService service,
        string nameOfAuthorEmail, string nameOfAuthor, string userEmail,
        string userIdentityName, string authorEmail)
    {
        if (IsInvalid(nameOfAuthor, modelState) &&
            IsInvalid(nameOfAuthorEmail, modelState))
        {
            return "Error";
        }

        service.CreateFollow(userIdentityName, userEmail, authorEmail);
        return "UserTimeline";
    }
    public static string Unfollow(ModelStateDictionary modelState, ICheepService service,
        string nameOfAuthorEmail, string nameOfAuthor, string userEmail,
        string authorEmail, SubmitMessageModel submitMessage)
    {
        if (IsInvalid(nameOfAuthor, modelState) &&
            IsInvalid(nameOfAuthorEmail, modelState))
        {
            if (submitMessage != null)
            {
                return "Page";
            }

            return "Error";
        }
        
        service.UnFollow(userEmail, authorEmail);
        return "Page";
    }
}