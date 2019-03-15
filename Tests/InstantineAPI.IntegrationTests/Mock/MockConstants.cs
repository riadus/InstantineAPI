using System;
using System.Text;
using InstantineAPI.Core;
using Microsoft.Extensions.Configuration;

namespace InstantineAPI.IntegrationTests.Mock
{
    public class MockConstants : IConstants
    {
        private readonly AppSettings _appsettings;
        public MockConstants(IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            _appsettings = appSettingsSection.Get<AppSettings>();
        }

        public string JwtEncryptionKey => _appsettings.Secret;

        public string AdminEmail => "mail@admin.io";

        public string AdminPwd => "WeakPwd!";

        public string PwdEncryptionKey => "akLz_Q3gQXOhdvvqP0UVz51gfHV+x4GF";

        public byte[] PwdSalt => Encoding.UTF8.GetBytes(PwdEncryptionKey);

        public int PwdIteration => 32;

        public object Iss => _appsettings.Iss;

        public object Aud => _appsettings.Aud;
    }
}
