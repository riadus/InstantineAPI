using System;
namespace InstantineAPI.Core
{
    public static class Constants
    {
        private static readonly string AppSettingsPrefix = "APPSETTING";

        public static string EncryptionKeyKey => $"{AppSettingsPrefix}_EncryptionKey";
        public static string EncryptionKey => Environment.GetEnvironmentVariable(EncryptionKeyKey);
    }
}
