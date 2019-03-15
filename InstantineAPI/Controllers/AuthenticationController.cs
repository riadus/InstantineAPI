using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using InstantineAPI.Core;
using InstantineAPI.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstantineAPI.Controllers
{
    [Authorize(AuthenticationSchemes = "BasicAuthentication")]
    [Route("api/[controller]")]
    public class AuthenticationController : BaseController
    {
        private readonly IConstants _constants;
        private readonly ITokenService _tokenService;

        public AuthenticationController(IUserService userService,
                                        IConstants constants,
                                        ITokenService tokenService) : base(userService)
        {
            _constants = constants;
            _tokenService = tokenService;
        }

        [HttpGet]
        public async Task<IActionResult> Authenticate()
        {
            var email = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.Email);
            var user = await GetUserByEmail(email.Value);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            var tokenString = _tokenService.GenerateTokenFor(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _userService.UpdateRefreshToken(user, refreshToken);

            return Ok(new
            {
                user.Id,
                user.UserId,
                user.FirstName,
                user.LastName,
                RefreshToken = refreshToken,
                Token = tokenString
            });
        }
    }
}
