using InstantineAPI.Core;
using InstantineAPI.Core.Domain;
using InstantineAPI.IntegrationTests.Mock;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace InstantineAPI.IntegrationTests
{
    public class InstantineApiTestServer : TestServerFixture
    {
        protected override TestServer CreateServer(IWebHostBuilder builder)
        {
            return base.CreateServer(builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IConstants, MockConstants>();
                services.AddSingleton<IRandomStringGenerator, FixedStringGenerator>();
            }));
        }
    }
}

