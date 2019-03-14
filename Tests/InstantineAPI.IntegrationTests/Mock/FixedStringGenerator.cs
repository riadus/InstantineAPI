using InstantineAPI.Core.Domain;

namespace InstantineAPI.IntegrationTests.Mock
{
    public class FixedStringGenerator : IRandomStringGenerator
    {
        public string GenerateString()
        {
            return "WeakPwd!";
        }
    }
}
