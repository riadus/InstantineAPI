using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
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
        [InlineData("yahia@algiers.dz", "Yahia", "Elchab")]
        [InlineData("jan@amsterdam.nl", "Jan", "de Jong")]
        public async Task RegisterManagers(string email, string firstName, string lastName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var users = await unitOfWork.Users.GetAll(x => x.Role == Data.UserRole.Manager);
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
            users = await unitOfWork.Users.GetAll(x => x.Role == Data.UserRole.Manager);
            users.Count().Should().Be(previousCount + 1);
        }

        [Theory, Order(3)]
        [InlineData("jean@paris.fr", "Jean", "Le Jeune")]
        [InlineData("giovani@roma.it", "Giovanni", "Il Giovane")]
        public async Task RegisterMembers(string email, string firstName, string lastName)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var users = await unitOfWork.Users.GetAll(x => x.Role == Data.UserRole.Member);
            var previousCount = users.Count();

            var userDto = new UserDto
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            var constants = _fixture.Services.GetRequiredService<IConstants>();
            var postResponse = await PostAsync(constants.AdminEmail, $"api/users/members", new ObjectContent<List<UserDto>>(new List<UserDto> { userDto }, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
            users = await unitOfWork.Users.GetAll(x => x.Role == Data.UserRole.Member);
            users.Count().Should().Be(previousCount + 1);
        }
    }
}