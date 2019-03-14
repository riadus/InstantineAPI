namespace InstantineAPI.Core.Domain
{
    public interface IEncryptionService
    {
        string StringEncrypt(string toEncrypt);
        string StringDecrypt(string toDecrypt);
    }
}
