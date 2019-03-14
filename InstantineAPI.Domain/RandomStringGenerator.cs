using System;
using System.Linq;
using InstantineAPI.Core.Domain;

namespace InstantineAPI.Domain
{
    public class RandomStringGenerator : IRandomStringGenerator
    {
        public string GenerateString()
        {
            var length = 16;
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-#?!+=";

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

    }
}