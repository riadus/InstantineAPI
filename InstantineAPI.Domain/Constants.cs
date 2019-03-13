using System;
using InstantineAPI.Core;
using Microsoft.Extensions.Configuration;

namespace InstantineAPI.Domain
{
    public class Constants : IConstants
    {
        private static readonly string AppSettingsPrefix = "APPSETTING";
        private readonly AppSettings _appsettings;
        public Constants(IConfiguration configuration)
        {
            var appSettingsSection = configuration.GetSection("AppSettings");
            _appsettings = appSettingsSection.Get<AppSettings>();
        }

        public string EncryptionKey => _appsettings.Secret;

        private string AdminEmailKey => $"{AppSettingsPrefix}_AdminEmailKey";
        public string AdminEmail => Environment.GetEnvironmentVariable(AdminEmailKey);

        private string AdminPwdKey => $"{AppSettingsPrefix}_AdminPwdKey";
        public string AdminPwd => Environment.GetEnvironmentVariable(AdminPwdKey);
    }
}