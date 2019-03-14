namespace InstantineAPI.Core.Domain
{
    public interface IPasswordService
    {
        (string Hash, string Salt) GenerateRandomPassword();
        (string Hash, string Salt) CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, string hash, string salt);
    }
}
