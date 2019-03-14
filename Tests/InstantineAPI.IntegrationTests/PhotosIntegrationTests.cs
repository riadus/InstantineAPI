using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.IntegrationTests.Configuration;
using Xunit;

namespace InstantineAPI.IntegrationTests
{
    [Order(4)]
    public class PhotosIntegrationTests : BaseIntegrationTests
    {
        public PhotosIntegrationTests(InstantineApiTestServer fixture) : base(fixture)
        {
        }

        [Theory, Order(1)]
        [InlineData("jan@amsterdam.nl", "Amsterdam", true)]
        [InlineData("jean@paris.fr", "Amsterdam", false)]
        [InlineData("giovani@roma.it", "Dzaïr", false)]
        [InlineData("yahia@algiers.dz", "Dzaïr", true)]
        public async Task AddPhoto(string publisherEmail, string albumName, bool successStatus)
        {
            var album = await GetAlbum(albumName);
            var publisher = await GetUserFromEmail(publisherEmail);
            var photoDto = new PhotoDto
            {
                UserId = publisher.UserId,
                Image = new byte[] { 0x12, 0x13, 0x14, 0x15 }
            };
            var postResponse = await PostAsync(publisherEmail, $"api/photo?albumId={album.AlbumId}", new ObjectContent<PhotoDto>(photoDto, _mediaTypeFormatter));
            postResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(2)]
        [InlineData("jan@amsterdam.nl", "Amsterdam", true)]
        [InlineData("jean@paris.fr", "Amsterdam", true)]
        [InlineData("giovani@roma.it", "Dzaïr", false)]
        [InlineData("yahia@algiers.dz", "Dzaïr", true)]
        public async Task SeePicture(string userEmail, string albumName, bool successStatus)
        {
            var album = await GetAlbum(albumName);
            var user = await GetUserFromEmail(userEmail);
            var photo = album.Photos.First();

            var getResponse = await GetAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            getResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(3)]
        [InlineData("jan@amsterdam.nl", "Amsterdam", true)]
        [InlineData("jean@paris.fr", "Amsterdam", false)]
        [InlineData("giovani@roma.it", "Dzaïr", false)]
        [InlineData("yahia@algiers.dz", "Dzaïr", true)]
        public async Task DeletePhoto(string userEmail, string albumName, bool successStatus)
        {
            var album = await GetAlbum(albumName);
            var photo = album.Photos.First();
            var user = await GetUserFromEmail(userEmail);
            var deleteResponse = await DeleteAsync(userEmail, $"api/photo/{photo.PhotoId}?albumId={album.AlbumId}");
            deleteResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }
    }
}
