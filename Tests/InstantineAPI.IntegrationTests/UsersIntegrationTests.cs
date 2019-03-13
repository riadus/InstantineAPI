using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using InstantineAPI.Controllers.Dtos;
using InstantineAPI.Core;
using InstantineAPI.Core.Database;
using InstantineAPI.IntegrationTests.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace InstantineAPI.IntegrationTests
{
    [Order(1)]
    public class UsersIntegrationTests : BaseIntegrationTests
    {
        public UsersIntegrationTests(InstantineApiTestServer fixture) : base(fixture)
        {

        }

        [Fact, Order(1)]
        public async Task Setup()
        {
            await _fixture.EnsureDatabaseEmpty();
        }

        [Theory, Order(2)]
        [InlineData("jean@paris.fr", "Jean", "Le Jeune")]
        [InlineData("jan@amsterdam.nl", "Jan", "de Jong")]
        [InlineData("giovani@roma.it", "Giovanni", "Il Giovane")]
        [InlineData("yahia@algiers.dz", "Yahia", "Elchab")]
        public async Task RegisterManagers(string email, string firstName, string lastName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var users = await unitOfWork.Users.GetAll();
            var previousCount = users.Count();

            var userDto = new UserDto
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            var constants = _fixture.Services.GetRequiredService<IConstants>();
            var postResponse = await PostAsync(constants.AdminEmail, $"api/users/manager", new ObjectContent<UserDto>(userDto, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
            users = await unitOfWork.Users.GetAll();
            var count = users.Count();
            Assert.Equal(previousCount, count - 1);
        }
    }
}