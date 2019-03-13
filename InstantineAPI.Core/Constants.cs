using System;
namespace InstantineAPI.Core
{
    public interface IConstants
    {
        string EncryptionKey { get; }
        string AdminEmail { get; }
        string AdminPwd { get; }
    }

}
