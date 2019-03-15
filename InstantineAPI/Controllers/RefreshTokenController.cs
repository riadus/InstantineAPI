using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using InstantineAPI.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace InstantineAPI.Controllers
{
    [Route("api/[controller]")]
    public class RefreshTokenController : BaseController
    {
        private readonly ITokenService _tokenService;

        public RefreshTokenController(IUserService userService, ITokenService tokenService) : base(userService)
        {
            _tokenService = tokenService;
        }

        [HttpPost]
        public async Task<IActionResult> Refresh([FromBody]Tokens tokens)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(tokens.Token);
            var userId = principal.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            var user = await _userService.GetUserFromId(userId);
            var validRefreshToken = _userService.VerifyRefreshToken(user, tokens.RefreshToken);
            if (!validRefreshToken)
                throw new SecurityTokenException("Invalid refresh token");

            var newJwtToken = _tokenService.GenerateTokenFor(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _userService.UpdateRefreshToken(user, newRefreshToken);

            return new ObjectResult(new Tokens
                {
                    Token = newJwtToken,
                    RefreshToken = newRefreshToken
                });
        }

        public class Tokens
        {
            public string Token { get; set; }
            public string RefreshToken { get; set; }
        }
    }
}
