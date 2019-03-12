using System.Net.Http;
using System.Threading.Tasks;
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

        [Fact, Order(0)]
        public async Task Setup()
        {
            await _fixture.EnsureDatabaseEmpty();
        }

        [Theory, Order(1)]
        [InlineData("jean@paris.fr", "Jean", "Le Jeune")]
        [InlineData("jan@amsterdam.nl", "Jan", "de Jong")]
        [InlineData("giovani@roma.it", "Giovanni", "Il Giovane")]
        [InlineData("yahia@algiers.dz", "Yahia", "Elchab")]
        public async Task SubscribeUsers(string email, string firstName, string lastName)
        {
            await SetupUserSubscription(email, firstName, lastName);
        }

        [Theory, Order(2)]
        [InlineData("jean@paris.fr", "Paris")]
        [InlineData("jean@paris.fr", "Plage")]
        [InlineData("jan@amsterdam.nl", "Amsterdam")]
        [InlineData("jan@amsterdam.nl", "Strand")]
        [InlineData("giovani@roma.it", "Roma")]
        [InlineData("giovani@roma.it", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "Dzaïr")]
        [InlineData("yahia@algiers.dz", "Chett")]
        public async Task CreateAlbum(string email, string albumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var users = await unitOfWork.Users.GetAll();

            var user = await GetUserFromEmail(email);
            var albumDto = new AlbumDto
            {
                Name = albumName
            };
            var postResponse = await PostAsync(email, $"api/album?requestorId={user.UserId}", new ObjectContent<AlbumDto>(albumDto, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
        }


        [Theory, Order(3)]
        [InlineData("jean@paris.fr", "jan@amsterdam.nl", "Amsterdam")]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Strand")]
        [InlineData("jan@amsterdam.nl", "giovani@roma.it", "Roma")]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Spiaggia")]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Dzaïr")]
        [InlineData("giovani@roma.it", "jean@paris.fr", "Chett")]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Paris")]
        [InlineData("yahia@algiers.dz", "jan@amsterdam.nl", "Plage")]
        public async Task AddFollower_Should_Fail(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var postResponse = await PostAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}", new ObjectContent<object>(null, _mediaTypeFormatter));
            Assert.False(postResponse.IsSuccessStatusCode);
        }

        [Theory, Order(4)]
        [InlineData("jean@paris.fr", "jan@amsterdam.nl", "Paris")]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Plage")]
        [InlineData("jan@amsterdam.nl", "giovani@roma.it", "Amsterdam")]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Strand")]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Roma")]
        [InlineData("giovani@roma.it", "jean@paris.fr", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr")]
        [InlineData("yahia@algiers.dz", "jan@amsterdam.nl", "Chett")]
        public async Task AddFollower(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var postResponse = await PostAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}", new ObjectContent<object>(null, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(5)]
        [InlineData("yahia@algiers.dz", "giovani@roma.it", "Plage")]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Strand")]
        [InlineData("jan@amsterdam.nl", "jean@paris.fr", "Spiaggia")]
        [InlineData("jean@paris.fr", "jan@amsterdam.nl", "Chett")]
        public async Task RemoveFollower_ShouldFail(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}");
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }

        [Theory, Order(6)]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Plage")]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Strand")]
        public async Task RemoveFollower_ByAdmin(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(7)]
        [InlineData("jean@paris.fr", "jean@paris.fr", "Spiaggia")]
        [InlineData("jan@amsterdam.nl", "jan@amsterdam.nl", "Chett")]
        public async Task RemoveFollower_BySelf(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/followers/{followerEmail}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(8)]
        [InlineData("jean@paris.fr", "jan@amsterdam.nl", "Paris")]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Plage")]
        [InlineData("jan@amsterdam.nl", "giovani@roma.it", "Amsterdam")]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Strand")]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Roma")]
        [InlineData("giovani@roma.it", "jean@paris.fr", "Spiaggia")]
        [InlineData("yahia@algiers.dz", "jean@paris.fr", "Dzaïr")]
        [InlineData("yahia@algiers.dz", "jan@amsterdam.nl", "Chett")]
        public async Task AddAdmin(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var postResponse = await PostAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}", new ObjectContent<object>(null, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(9)]
        [InlineData("yahia@algiers.dz", "giovani@roma.it", "Plage")]
        [InlineData("giovani@roma.it", "yahia@algiers.dz", "Strand")]
        [InlineData("jan@amsterdam.nl", "jean@paris.fr", "Spiaggia")]
        [InlineData("jean@paris.fr", "jan@amsterdam.nl", "Chett")]
        public async Task RemoveAdmin_ShouldFail(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}");
            Assert.False(deleteResponse.IsSuccessStatusCode);
        }

        [Theory, Order(10)]
        [InlineData("jean@paris.fr", "giovani@roma.it", "Plage")]
        [InlineData("jan@amsterdam.nl", "yahia@algiers.dz", "Strand")]
        public async Task RemoveAdmin_ByAdmin(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}");
            deleteResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(11)]
        [InlineData("jean@paris.fr", "jean@paris.fr", "Spiaggia")]
        [InlineData("jan@amsterdam.nl", "jan@amsterdam.nl", "Chett")]
        [InlineData("jan@amsterdam.nl", "jan@amsterdam.nl", "Paris")]
        public async Task RemoveAdmin_BySelf(string requestorEmail, string followerEmail, string AlbumName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var album = await unitOfWork.Albums.GetFirst(x => x.Name == AlbumName);

            var requestor = await GetUserFromEmail(requestorEmail);
            var deleteResponse = await DeleteAsync(requestorEmail, $"api/album/{album.AlbumId}/admins/{followerEmail}");
            deleteResponse.EnsureSuccessStatusCode();
        }
    }
}