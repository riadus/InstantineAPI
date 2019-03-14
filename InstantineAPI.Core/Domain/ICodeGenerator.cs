namespace InstantineAPI.Core.Domain
{
    public interface ICodeGenerator
    {
        byte[] GenerateImageFromCode(string code);
    }
}
