using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;

namespace InstantineAPI.Controllers
{
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UserController(IUserService userService,
                              IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> CreateUsers([FromBody]List<UserDto> usersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var users = usersDto.Select(userDto => _mapper.Map<User>(userDto));
                await _userService.SubscribeUsers(users);
                return Ok();
            }
        }

        [HttpPost]
        [Route("send")]
        public async Task<IActionResult> SendEmail()
        {
            await _userService.SendEmailToUsers();
            return Ok();
        }

        [HttpPost]
        [Route("resend")]
        public async Task<IActionResult> ResendEmail()
        {
            await _userService.SendAgainEmailToUsers();
            return Ok();
        }

        [HttpPost]
        [Route("accept/{code}")]
        public async Task<IActionResult> AcceptInvitation(string code)
        {
            await _userService.AcceptInvitation(code);
            return Ok();
        }
    }
}
