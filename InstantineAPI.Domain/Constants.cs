using System;
using System.Text;
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

        public string JwtEncryptionKey => _appsettings.Secret;

        private string AdminEmailKey => $"{AppSettingsPrefix}_AdminEmailKey";
        public string AdminEmail => Environment.GetEnvironmentVariable(AdminEmailKey);

        private string AdminPwdKey => $"{AppSettingsPrefix}_AdminPwdKey";
        public string AdminPwd => Environment.GetEnvironmentVariable(AdminPwdKey);

        public string PwdEncryptionKeyKey => $"{AppSettingsPrefix}_PwdEncryptionKeyKey";
        public string PwdEncryptionKey => Environment.GetEnvironmentVariable(PwdEncryptionKeyKey);

        public string PwdSaltKey => $"{AppSettingsPrefix}_PwdSaltKey";
        public byte[] PwdSalt => Encoding.UTF8.GetBytes(PwdEncryptionKey);

        public string PwdIterationKey => $"{AppSettingsPrefix}_PwdIterationKey";
        public int PwdIteration => int.Parse(Environment.GetEnvironmentVariable(PwdIterationKey));

        public string Iss => _appsettings.Iss;
        public string Aud => _appsettings.Aud;
    }
}