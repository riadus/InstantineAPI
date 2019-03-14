using System.Collections.Generic;
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
    public class UserIntegrationTest : BaseIntegrationTests
    {
        public UserIntegrationTest(InstantineApiTestServer fixture) : base(fixture)
        {
        }

        [Fact, Order(1)]
        public async Task Setup()
        {
            await _fixture.EnsureDatabaseEmpty();
        }

        [Theory, Order(2)]
        [InlineData("john@mail.com", "Jean", "Le Jeune")]
        [InlineData("jane@mail.com", "Jeanne", "La Jeune")]
        public async Task RegisterMembers(string email, string firstName, string lastName)
        {
            var userDto = new UserDto
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName
            };
            var constants = _fixture.Services.GetRequiredService<IConstants>();
            var postResponse = await PostAsync(constants.AdminEmail, $"api/administration/members", new ObjectContent<List<UserDto>>(new List<UserDto> { userDto }, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();
        }

        [Theory, Order(3)]
        [InlineData("john@mail.com", "$trong_P4ssw0rd!", true)]
        [InlineData("jane@mail.com", "tooweak", false)]
        public async Task ChangePassword(string email, string password, bool successStatus)
        {
            var userDto = new UserDto
            {
                Email = email,
                PasswordDto = new PasswordDto { Password = password }
            };
            var postResponse = await PostAsync(email, $"api/user", new ObjectContent<UserDto>(userDto, _mediaTypeFormatter));
            postResponse.IsSuccessStatusCode.Should().Be(successStatus);
        }

        [Theory, Order(4)]
        [InlineData("john@mail.com", "Yahia", "Ecchab", "$trong_P4ssw0rd!")]
        public async Task ChangeNames(string email, string newFirstName, string newLastName, string password)
        {
            var userDto = new UserDto
            {
                Email = email,
                FirstName = newFirstName,
                LastName = newLastName
            };
            var token = await GetToken(email, password);
            var client = _fixture.GetClient(token);
            var postResponse = await client.PostAsync($"api/user", new ObjectContent<UserDto>(userDto, _mediaTypeFormatter));
            postResponse.EnsureSuccessStatusCode();

            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var user = await unitOfWork.Users.GetFirst(x => x.Email == email);
            user.FirstName.Should().Be(newFirstName);
            user.LastName.Should().Be(newLastName);
        }
    }
}