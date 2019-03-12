using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using Microsoft.AspNetCore.Authorization;
using InstantineAPI.Core.Photos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InstantineAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ReactionController : BaseController
    {
        private readonly IReactionService _reactionService;

        public ReactionController(IReactionService reactionService, IUserService userService) : base(userService)
        {
            _reactionService = reactionService;
        }

        [HttpPost]
        [Route("comment")]
        public async Task<IActionResult> AddComment([FromBody]CommentDto commentDto, [FromQuery] string photoId, [FromQuery] string albumId)
        {
            var reactor = await GetUser();
            if (reactor == null)
            {
                return BadRequest();
            }
            var comment = await _reactionService.AddComment(photoId, albumId, commentDto.Message, reactor);
            return Ok(comment);
        }

        [HttpDelete]
        [Route("comment")]
        public async Task<IActionResult> RemoveComment([FromQuery]string photoId, [FromQuery]string commentId, [FromQuery] string albumId)
        {
            var reactor = await GetUser();
            if (reactor == null)
            {
                return BadRequest();
            }
            await _reactionService.DeleteComment(photoId, albumId, commentId, reactor);
            return Ok();
        }

        [HttpPost]
        [Route("like")]
        public async Task<IActionResult> AddLike([FromQuery]string photoId, [FromQuery] string albumId)
        {
            var reactor = await GetUser();
            if (reactor == null)
            {
                return BadRequest();
            }
            var like = await _reactionService.LikePhoto(photoId, albumId, reactor);
            return Ok(like);
        }

        [HttpDelete]
        [Route("like")]
        public async Task<IActionResult> RemoveLike([FromQuery]string photoId, [FromQuery] string albumId)
        {
            var reactor = await GetUser();
            if (reactor == null)
            {
                return BadRequest();
            }
            await _reactionService.UnlikePhoto(photoId, albumId, reactor);
            return Ok();
        }
    }
}