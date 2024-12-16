using Chirp.Core;
using Chirp.Web.Pages.Partials;

using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Chirp.Web.Pages.Utils;

public static class FollowHandler
{
    public static string Follow(ModelStateDictionary modelState, IAuthorService service,
        string nameOfAuthorEmail, string nameOfAuthor, string userEmail,
        string userIdentityName, string authorEmail)
    {
        if (StateValidator.IsInvalid(nameOfAuthor, modelState) &&
            StateValidator.IsInvalid(nameOfAuthorEmail, modelState))
        {
            return "Error";
        }

        service.CreateFollow(userIdentityName, userEmail, authorEmail);
        return "UserTimeline";
    }
    // TODO: FIX THIS
    public static string Unfollow(ModelStateDictionary modelState, IAuthorService service,
        string nameOfAuthorEmail, string nameOfAuthorUsername, string username,
        string authorUsername, SubmitMessageModel submitMessage)
    {
        if (StateValidator.IsInvalid(nameOfAuthorUsername, modelState) &&
            StateValidator.IsInvalid(nameOfAuthorEmail, modelState))
        {
            if (submitMessage != null)
            {
                return "Page";
            }

            return "Error";
        }
        
        service.UnFollow(username, authorUsername);
        return "Page";
    }
    
}