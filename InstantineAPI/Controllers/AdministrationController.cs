using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using InstantineAPI.Middelware.Attributes;

namespace InstantineAPI.Controllers
{
    [Route("api/administration")]
    [AuthorizeAdminOnly]
    public class AdministrationController : BaseController
    {
        private readonly IMapper _mapper;

        public AdministrationController(IUserService userService,
                              IMapper mapper) : base(userService)
        {
            _mapper = mapper;
        }

        [HttpPost("members")]
        public async Task<IActionResult> CreateMember([FromBody]List<UserDto> usersDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                var users = usersDto.Select(userDto => _mapper.Map<User>(userDto));
                await _userService.RegisterMembers(users);
                return Ok();
            }
        }

        [HttpPost("manager")]
        public async Task<IActionResult> CreateManager([FromBody]UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                await _userService.RegisterManager(_mapper.Map<User>(userDto));
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
