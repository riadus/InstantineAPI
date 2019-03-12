using InstantineAPI.Controllers.Dtos;
using System.Threading.Tasks;
using InstantineAPI.IntegrationTests.Configuration;
using Xunit;
using System.Net.Http;
using System.Net;
using InstantineAPI.Data;
using System.Linq;

namespace InstantineAPI.IntegrationTests
{
    [Order(3)]
    public class PhotosIntegrationTests : BaseIntegrationTests
    {
        public PhotosIntegrationTests(InstantineApiTestServer fixture) : base(fixture)
        {
        }

        [Theory, Order(1)]
        [InlineData("jan@amsterdam.nl", "Plage")]
        [InlineData("jean@paris.fr", "Strand")]
        [InlineData("giovani@roma.it", "Chett")]
        [InlineData("yahia@algiers.dz", "Spiaggia")]
        public async Task AddPhoto_ShouldFail(string publisherEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var publisher = await GetUserFromEmail(publisherEmail);
            var photoDto = new PhotoDto
            {
                UserId = publisher.UserId,
                Image = new byte[] { 0x12, 0x13, 0x14, 0x15 }
            };
            var postResponse = await PostAsync(publisherEmail, $"api/photo?albumId={album.AlbumId}", new ObjectContent<PhotoDto>(photoDto, _mediaTypeFormatter));
            Assert.False(postResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, postResponse.StatusCode);
        }

        [Theory, Order(2)]
        [InlineData("jan@amsterdam.nl", "Strand")]
        [InlineData("jean@paris.fr", "Plage")]
        [InlineData("jean@paris.fr", "Paris")]
        [InlineData("giovani@roma.it", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "Chett")]
        public async Task AddPhoto_ByCreator(string publisherEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var publisher = await GetUserFromEmail(publisherEmail);
            var photoDto = new PhotoDto
            {
                UserId = publisher.UserId,
                Image = new byte[] { 0x12, 0x13, 0x14, 0x15 }
            };
            var postResponse = await PostAsync(publisherEmail, $"api/photo?albumId={album.AlbumId}", new ObjectContent<PhotoDto>(photoDto, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
            var photo = await postResponse.Content.ReadAsAsync<Photo>();
        }

        [Theory, Order(3)]
        [InlineData("jean@paris.fr", "Dzaïr")]
        [InlineData("giovani@roma.it", "Amsterdam")]
        [InlineData("yahia@algiers.dz", "Roma")]
        public async Task AddPhoto_ByAdmin(string publisherEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var publisher = await GetUserFromEmail(publisherEmail);
            var photoDto = new PhotoDto
            {
                UserId = publisher.UserId,
                Image = new byte[] { 0x12, 0x13, 0x14, 0x15 }
            };
            var postResponse = await PostAsync(publisherEmail, $"api/photo?albumId={album.AlbumId}", new ObjectContent<PhotoDto>(photoDto, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
            var photo = await postResponse.Content.ReadAsAsync<Photo>();
        }


        [Theory, Order(4)]
        [InlineData("jan@amsterdam.nl", "Chett")]
        [InlineData("jean@paris.fr", "Spiaggia")]
        [InlineData("giovani@roma.it", "Plage")]
        [InlineData("yahia@algiers.dz", "Strand")]
        public async Task SeePicture_ShoudFail(string userEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var user = await GetUserFromEmail(userEmail);
            var photo = album.Photos.First();

            var getResponse = await GetAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            Assert.False(getResponse.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Theory, Order(5)]
        [InlineData("jan@amsterdam.nl", "Paris")] // Follower
        [InlineData("jean@paris.fr", "Plage")] // Admin
        [InlineData("giovani@roma.it", "Spiaggia")] // Creator
        public async Task SeePicture(string userEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var photo = album.Photos.First();
            var user = await GetUserFromEmail(userEmail);
            var getResponse = await GetAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            getResponse.EnsureSuccessStatusCode();
            var photoBytes = await getResponse.Content.ReadAsAsync<byte[]>();
            var expectedBytes = new byte[] { 0x12, 0x13, 0x14, 0x15 };
            Assert.Equal(photoBytes, expectedBytes);
        }

        [Theory, Order(6)]
        [InlineData("jan@amsterdam.nl", "Paris")] // Follower
        [InlineData("yahia@algiers.dz", "Paris")] // Not Follower
        public async Task DeletePhoto_ShoudFail(string userEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var photo = album.Photos.First();
            var user = await GetUserFromEmail(userEmail);
            var deleteResponse = await DeleteAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }

        [Theory, Order(7)]
        [InlineData("jan@amsterdam.nl", "Strand")]
        [InlineData("jean@paris.fr", "Plage")]
        [InlineData("giovani@roma.it", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "Chett")]
        public async Task DeletePhoto_ByCreator(string userEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var photo = album.Photos.First();
            var user = await GetUserFromEmail(userEmail);
            var deleteResponse = await DeleteAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(8)]
        [InlineData("jan@amsterdam.nl", "Strand")]
        [InlineData("jean@paris.fr", "Plage")]
        [InlineData("giovani@roma.it", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "Chett")]
        public async Task DeletePhoto_ByAdmin(string userEmail, string albumName)
        {
            var album = await GetAlbum(albumName);
            var photo = album.Photos.First();
            var user = await GetUserFromEmail(userEmail);
            var deleteResponse = await DeleteAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }
    }
}
