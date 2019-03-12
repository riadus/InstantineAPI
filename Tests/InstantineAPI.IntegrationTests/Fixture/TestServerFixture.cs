using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using InstantineAPI.Core.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InstantineAPI.IntegrationTests
{
    public abstract class TestServerFixture : IDisposable
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        protected TestServerFixture()
        {
            TestServer = CreateServer(new WebHostBuilder());

            Client = TestServer.CreateClient();
        }

        public TestServer TestServer { get; }

        public HttpClient Client { get; set; }

        public HttpClient GetClient(string token)
        {
            var client = TestServer.CreateClient();
            client.DefaultRequestHeaders.Remove("Authorization");
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return client;
        }

        public IServiceProvider Services => TestServer.Host.Services;

        protected virtual TestServer CreateServer(IWebHostBuilder builder)
        {
            return new TestServer(
                builder
                    .UseConfiguration(Configuration)
                    .UseStartup<Startup>()
            );
        }

        public async Task EnsureDatabaseEmpty()
        {
            var unitOfWork = Services.GetRequiredService<IUnitOfWork>();
            await unitOfWork.Likes.Delete(x => true);
            await unitOfWork.Comments.Delete(x => true);
            await unitOfWork.Photos.Delete(x => true);
            await unitOfWork.Albums.Delete(x => true);
            await unitOfWork.Users.Delete(x => true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                TestServer?.Dispose();
                Client?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
