using System;
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

        public string EncryptionKey => _appsettings.Secret;

        public string AdminEmail => "mail@admin.io";

        public string AdminPwd => "WeakPwd!";
    }
}
