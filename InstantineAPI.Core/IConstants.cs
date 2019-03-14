using System;
namespace InstantineAPI.Core
{
    public interface IConstants
    {
        string JwtEncryptionKey { get; }
        string PwdEncryptionKey { get; }
        byte[] PwdSalt { get; }
        int PwdIteration { get; }
        string AdminEmail { get; }
        string AdminPwd { get; }
    }

}
