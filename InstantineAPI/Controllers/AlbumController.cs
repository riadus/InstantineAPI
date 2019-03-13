using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Domain;
using InstantineAPI.Core.Photos;
using Microsoft.AspNetCore.Authorization;
using InstantineAPI.Middelware.Attributes;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace InstantineAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class AlbumController : BaseController
    {
        private readonly IPhotoService _photoService;
        private readonly IAlbumService _albumService;
        private readonly IMapper _mapper;

        public AlbumController(IPhotoService photoService,
                               IAlbumService albumService,
                               IUserService userService,
                               IMapper mapper) : base(userService)
        {
            _photoService = photoService;
            _albumService = albumService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAlbums()
        {
            var requestor = await GetUser();
            return Ok(await _albumService.GetAlbums(requestor));
        }

        [HttpPost]
        [AuthorizeManager]
        public async Task<IActionResult> CreateAlbum([FromBody]AlbumDto albumDto)
        {
            var requestor = await GetUser();
            var album = await _albumService.CreateAlbum(requestor, albumDto.Name);
            return Ok(album);
        }

        [HttpGet]
        [Route("{albumId}")]
        public async Task<IActionResult> GetPhotos([FromRoute]string albumId)
        {
            var requestor = await GetUser();
            var photos = await _photoService.GetPhotos(albumId, requestor);
            return Ok(photos);
        }

        [HttpPost]
        [Route("{albumId}/followers/{followerEmail}")]
        public async Task<IActionResult> AddFollower([FromRoute]string albumId, [FromRoute]string followerEmail)
        {
            var requestor = await GetUser();
            var follower = await GetUserByEmail(followerEmail);
            if (await _albumService.AddFollower(albumId, follower, requestor))
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("{albumId}/followers")]
        public async Task<IActionResult> GetFollowers([FromRoute]string albumId)
        {
            return Ok(await _albumService.GetFollowers(albumId));
        }

        [HttpGet]
        [Route("{albumId}/admins")]
        public async Task<IActionResult> GetAdmins([FromRoute]string albumId)
        {
            return Ok(await _albumService.GetAdmins(albumId));
        }

        [HttpDelete]
        [Route("{albumId}/followers/{followerEmail}")]
        public async Task<IActionResult> RemoveFollower([FromRoute]string albumId, [FromRoute]string followerEmail)
        {
            var requestor = await GetUser();
            var follower = await GetUserByEmail(followerEmail);
            if (await _albumService.RemoveFollower(albumId, follower, requestor))
            {
                return Ok();
            }
            return BadRequest();
        }

        [HttpPost]
        [Route("{albumId}/admins/{adminEmail}")]
        public async Task<IActionResult> AddAdmin([FromRoute]string albumId, [FromRoute]string adminEmail)
        {
            var requestor = await GetUser();
            var admin = await GetUserByEmail(adminEmail);
            if (await _albumService.AddAdmin(albumId, admin, requestor))
            {
                return StatusCode(StatusCodes.Status201Created);
            }
            return BadRequest();
        }

        [HttpDelete]
        [Route("{albumId}/admins/{adminEmail}")]
        public async Task<IActionResult> RemoveAdmin([FromRoute]string albumId, [FromRoute]string adminEmail)
        {
            var requestor = await GetUser();
            var admin = await GetUserByEmail(adminEmail);
            if (await _albumService.RemoveAdmin(albumId, admin, requestor))
            {
                return Ok();
            }
            return BadRequest();
        }
    }
}
