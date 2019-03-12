using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Photos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InstantineAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PhotoController : BaseController
    {
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public PhotoController(IPhotoService photoService,
                               IUserService userService,
                               IMapper mapper) : base(userService)
        {
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> PostPhoto([FromBody]PhotoDto photoDto, [FromQuery]string albumId)
        {
            var author = await GetUser();
            var photo = await _photoService.StorePhoto(photoDto.Image, author, albumId);
            if(photo == null)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }
            return Ok(photo);
        }

        [HttpGet]
        [Route("{photoId}")]
        public async Task<IActionResult> GetPhoto([FromRoute]string photoId, [FromQuery]string albumId)
        {
            var requestor = await GetUser();
            var photo = await _photoService.GetPicture(photoId, albumId, requestor);
            if (photo == null)
            {
                return NotFound();
            }
            return Ok(photo);
        }

        [HttpDelete]
        [Route("{photoId}")]
        public async Task<IActionResult> DeletePhoto([FromRoute]string photoId, [FromQuery]string albumId)
        {
            var requestor = await GetUser();
            if (await _photoService.DeletePhoto(photoId, albumId, requestor))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
