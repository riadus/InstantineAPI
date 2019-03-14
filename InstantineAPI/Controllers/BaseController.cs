using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using System.Linq;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InstantineAPI.Controllers
{
    public abstract class BaseController : Controller
    {
        protected readonly IUserService _userService;

        protected BaseController(IUserService userService)
        {
            _userService = userService;
        }

        protected Task<User> GetUser()
        {
            var userId = HttpContext.User.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
            return _userService.GetUserFromId(userId);
        }

        protected Task<User> GetUserByEmail(string email)
        {
            return _userService.GetUserFromEmail(email);
        }
    }
}
