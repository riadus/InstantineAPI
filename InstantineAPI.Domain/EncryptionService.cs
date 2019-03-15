using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using InstantineAPI.Core;
using InstantineAPI.Core.Domain;

namespace InstantineAPI.Domain
{
    public class EncryptionService : IEncryptionService
    {
        private readonly IConstants _constants;

        public EncryptionService(IConstants constants)
        {
            _constants = constants;
        }

        public string StringEncrypt(string inText)
        {
            var bytesBuff = Encoding.Unicode.GetBytes(inText);
            using (var aes = Aes.Create())
            {
                var crypto = new Rfc2898DeriveBytes(_constants.PwdEncryptionKey, _constants.PwdSalt, _constants.PwdIteration);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    inText = Convert.ToBase64String(mStream.ToArray());
                }
            }
            return inText;
        }

        public string StringDecrypt(string cryptTxt)
        {
            cryptTxt = cryptTxt.Replace(" ", "+");
            var bytesBuff = Convert.FromBase64String(cryptTxt);
            using (var aes = Aes.Create())
            {
                var crypto = new Rfc2898DeriveBytes(_constants.PwdEncryptionKey, _constants.PwdSalt, _constants.PwdIteration);
                aes.Key = crypto.GetBytes(32);
                aes.IV = crypto.GetBytes(16);
                using (var mStream = new MemoryStream())
                {
                    using (var cStream = new CryptoStream(mStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cStream.Write(bytesBuff, 0, bytesBuff.Length);
                        cStream.Close();
                    }
                    cryptTxt = Encoding.Unicode.GetString(mStream.ToArray());
                }
            }
            return cryptTxt;
        }
    }
}