using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Zlo4NET.Api.Models.Shared
{
    public static class ZServerPassword
    {
        #region Helpers

        private static byte[] _stringToBytes(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }

        #endregion

        // thanks Aim4Kill for link
        // https://pastebin.com/MRTwreLn
        public static bool Verify(string secret, string password)
        {
            var parts = secret.Split(':');
            var salt = _stringToBytes(parts[0]);
            var originalHash = _stringToBytes(parts[1]);

            // ida pro tells us that password max size can be 124, so respectively data size can be 128 max
            int dataSize = password.Length;
            if (dataSize > 124)
            {
                dataSize = 124;
            }

            dataSize += 4;

            var buffer = new byte[dataSize];
            Buffer.BlockCopy(salt, 0, buffer, 0, 4);

            var passwordBytes = Encoding.ASCII.GetBytes(password);
            Buffer.BlockCopy(passwordBytes, 0, buffer, 4, dataSize - 4);

            var resultingHash = new SHA1CryptoServiceProvider()
                .ComputeHash(buffer);

            for (int i = 0; i < originalHash.Length; i++)
                if (originalHash[i] != resultingHash[i]) return false;

            return true;
        }
    }
}
