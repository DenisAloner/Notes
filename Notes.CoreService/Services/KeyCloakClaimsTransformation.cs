using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Notes.CoreService.Options;

namespace Notes.CoreService.Services;

public class KeyCloakClaimsTransformation : IClaimsTransformation
{
    private readonly string _clientId;

    public KeyCloakClaimsTransformation(IOptions<KeyCloakOptions> options)
    {
        _clientId = options.Value.ClientId;
    }

    public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        if (principal.Identity is ClaimsIdentity { IsAuthenticated: true } claimsIdentity)
        {
            var resourceAccessClaim = principal.FindFirst(claim => claim.Type == "resource_access");
            if (resourceAccessClaim != null)
            {
                var json = JsonSerializer.Deserialize<JsonElement>(resourceAccessClaim.Value);
                var role = json.EnumerateObject()
                    .First(x => x.NameEquals(_clientId)).Value.EnumerateObject()
                    .First(x => x.NameEquals("roles")).Value.EnumerateArray()
                    .Select(x => x.GetString())
                    .FirstOrDefault();
                if (role != null)
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }

        return Task.FromResult(principal);
    }
}