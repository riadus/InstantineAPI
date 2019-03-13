using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using InstantineAPI.Core.Database;
using InstantineAPI.Core.Domain;
using InstantineAPI.Data;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace InstantineAPI.IntegrationTests
{
    public abstract class BaseIntegrationTests : IClassFixture<InstantineApiTestServer>
    {
        protected BaseIntegrationTests(InstantineApiTestServer fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _mediaTypeFormatter = new JsonMediaTypeFormatter();
        }

        protected readonly JsonMediaTypeFormatter _mediaTypeFormatter;
        protected readonly InstantineApiTestServer _fixture;

        protected async Task<User> GetUserFromEmail(string email)
        {
            var userService = _fixture.Services.GetRequiredService<IUserService>();
            return await userService.GetUserFromEmail(email);
        }

        protected async Task SetupUserSubscription(string email, string firstName, string lastName)
        {
            var userService = _fixture.Services.GetRequiredService<IUserService>();
            var user = new User { Email = email, FirstName = firstName, LastName = lastName };
            await userService.RegisterMembers(new List<User> { user });
        }

        protected Task<Album> GetAlbum(string name)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            return unitOfWork.Albums.GetFirst(x => x.Name == name);
        }

        protected async Task<string> GetToken(string email)
        {
            var unitOfWork = _fixture.Services.GetRequiredService<IUnitOfWork>();
            var user = await unitOfWork.Users.GetFirst(x => x.Email == email);

            var base64 = Base64Encode($"{email}:{user.Code}");

            _fixture.Client.DefaultRequestHeaders.Remove("Authorization");
            _fixture.Client.DefaultRequestHeaders.Add("Authorization", $"Basic {base64}");
            var getReponse = await _fixture.Client.GetAsync("api/authentication");
            var authDto = await getReponse.Content.ReadAsAsync<AuthDto>();
            return authDto.Token;
        }

        private class AuthDto
        {
            public string Token { get; set; }
        }

        private Dictionary<string, HttpClient> _clients = new Dictionary<string, HttpClient>();

        protected async Task<HttpClient> GetAuhorizedClient(string email)
        {
            if(!_clients.ContainsKey(email))
            {
                var token = await GetToken(email);
                _clients.Add(email, _fixture.GetClient(token));
            }

            return _clients[email];
        }

        protected async Task<HttpResponseMessage> PostAsync(string email, string url, HttpContent content)
        {
            var client = await GetAuhorizedClient(email);
            return await client.PostAsync(url, content);
        }

        protected async Task<HttpResponseMessage> GetAsync(string email, string url)
        {
            var client = await GetAuhorizedClient(email);
            return await client.GetAsync(url);
        }

        protected async Task<HttpResponseMessage> DeleteAsync(string email, string url)
        {
            var client = await GetAuhorizedClient(email);
            return await client.DeleteAsync(url);
        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
