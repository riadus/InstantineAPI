using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core.Database;
using InstantineAPI.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace InstantineAPI.IntegrationTests
{
    [Order(2)]
    public class AlbumIntegrationTests : BaseIntegrationTests
    {
        public AlbumIntegrationTests(InstantineApiTestServer fixture) : base(fixture)
        {
        }

        [Theory, Order(1)]
        [InlineData("jean@paris.fr", "Paris", false)]
        [InlineData("jan@amsterdam.nl", "Amsterdam", true)]
        [InlineData("giovani@roma.it", "Roma", false)]
        [InlineData("yahia@algiers.dz", "Dzaïr", true)]
        public async Task CreateAlbum(string email, string albumName, bool successStatus)
        {
            var user = await GetUserFromEmail(email);
            var albumDto = new AlbumDto
            {
                Name = albumName
            };
            var postResponse = await PostAsync(email, $"api/album?requestorId={user.UserId}", new ObjectContent<AlbumDto>(albumDto, _mediaTypeFormatter));
            postResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }


        [Theory, Order(2)]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Amsterdam", false)]
        [InlineData("jan@amsterdam.nl", "giovani@roma.it", "Amsterdam", true)]
        [InlineData("jan@amsterdam.nl", "jean@paris.fr", "Amsterdam", true)]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Dzaïr", false)]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr", true)]
        public async Task AddFollower(string requestorEmail, string followerEmail, string albumName, bool successStatus)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == albumName);

            var postResponse = await PostAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}", new ObjectContent<object>(null, _mediaTypeFormatter));
            postResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(3)]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr", true)]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Amsterdam", false)]
        [InlineData("giovani@roma.it", "giovani@roma.it", "Amsterdam", true)]
        public async Task RemoveFollower(string requestorEmail, string followerEmail, string albumName, bool successStatus)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == albumName);

            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}");
            deleteResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(4)]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Amsterdam", false)]
        [InlineData("jan@amsterdam.nl", "giovani@roma.it", "Amsterdam", true)]
        [InlineData("jan@amsterdam.nl", "jean@paris.fr", "Amsterdam", true)]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Dzaïr", false)]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr", true)]
        public async Task AddAdmin(string requestorEmail, string followerEmail, string albumName, bool successStatus)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == albumName);

            var postResponse = await PostAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}", new ObjectContent<object>(null, _mediaTypeFormatter));
            postResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(5)]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr", true)]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Amsterdam", false)]
        [InlineData("jean@paris.fr", "jean@paris.fr", "Amsterdam", true)]
        public async Task RemoveAdmin(string requestorEmail, string followerEmail, string albumName, bool successStatus)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == albumName);

            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}");
            deleteResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }
    }
}