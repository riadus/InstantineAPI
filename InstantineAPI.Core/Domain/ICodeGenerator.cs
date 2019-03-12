namespace InstantineAPI.Core.Domain
{
    public interface ICodeGenerator
    {
        string GenerateRandomCode();
        byte[] GenrateImageFromCode(string code);
    }
}
