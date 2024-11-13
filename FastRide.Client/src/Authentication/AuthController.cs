using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FastRide.Client.Authentication;

public class AuthController : Controller
{
    [HttpGet("login-google")]
    public IActionResult Login([FromQuery] string returnUrl)
    {
        var redirectUri = string.IsNullOrEmpty(returnUrl)
            ? Url.Content("~/")
            : $"/{returnUrl}";

        if (User.Identity is { IsAuthenticated: true })
        {
            return LocalRedirect(redirectUri);
        }

        return Challenge(new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleSigninCallBack)) + returnUrl
        }, GoogleDefaults.AuthenticationScheme);
        
        /*var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action(nameof(GoogleResponse)) +  returnUrl,
        };
        return Challenge(properties,  GoogleDefaults.AuthenticationScheme);*/
    }

    [HttpGet("login-google-callback")]
    public IActionResult GoogleSigninCallBack([FromQuery] string returnUrl)
    {
        var redirectUri = string.IsNullOrEmpty(returnUrl)
            ? Url.Content("~/")
            : $"/{returnUrl}";

        return LocalRedirect(redirectUri);
    }
    
    /*[HttpGet("google-response")]
    [Authorize(AuthenticationSchemes = "Google")]
    public IActionResult GoogleResponse()
    {
        // handle what you need from auth object
        // e.g. create and return your own Bearer token from email
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        // ...
        
        return LocalRedirect("/");
    }*/
}