using System.Security.Claims;
using InstantineAPI.Data;

namespace InstantineAPI.Core.Domain
{
    public interface ITokenService
    {
        string GenerateTokenFor(User user);
        string GenerateRefreshToken();
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
