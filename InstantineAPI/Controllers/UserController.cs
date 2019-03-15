using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using Microsoft.AspNetCore.Authorization;

namespace InstantineAPI.Controllers
{
    [Route("api/user")]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IMapper _mapper;

        public UserController(IUserService userService, IMapper mapper) : base(userService)
        {
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUser([FromBody] UserDto userDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState.Values);
            }
            var user = await GetUser();
            if(user.Email != userDto.Email)
            {
                return BadRequest();
            }
            await _userService.ChangeUser(user, _mapper.Map<UserChangeRequest>(userDto));
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSelf()
        {
            return Ok(await GetUser());
        }
    }
}
