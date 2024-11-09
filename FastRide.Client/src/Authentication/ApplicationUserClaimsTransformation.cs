using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

public class ApplicationUserClaimsTransformation : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = principal.Identities.FirstOrDefault(c => c.IsAuthenticated);
        if (identity == null) return principal;

        //var user = await _userManager.GetUserAsync(principal);
        if (user == null) return principal;

        // Add or replace identity.Claims.


        if (!principal.HasClaim(c => c.Type == ClaimTypes.GivenName))
        {
            identity.AddClaim(new Claim(ClaimTypes.GivenName, user.FirstName));
        }

        if (!principal.HasClaim(c => c.Type == ClaimTypes.Surname))
        {
            identity.AddClaim(new Claim(ClaimTypes.Surname, user.LastName));
        }

        return new ClaimsPrincipal(identity);
    }
}