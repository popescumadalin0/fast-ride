using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;

namespace FastRide_Client.Authentication;

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
    }

    [HttpGet("login-google-callback")]
    public IActionResult GoogleSigninCallBack([FromQuery] string returnUrl)
    {
        var redirectUri = string.IsNullOrEmpty(returnUrl)
            ? Url.Content("~/")
            : $"/{returnUrl}";

        return LocalRedirect(redirectUri);
    }
}