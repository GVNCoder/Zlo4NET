using System;
using System.Linq;
using System.Text;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZBitConverter
    {
        public static byte[] Convert(int value) => BitConverter.GetBytes(value).Reverse().ToArray();
        public static byte[] Convert(uint value) => BitConverter.GetBytes(value).Reverse().ToArray();
        public static byte[] Convert(ushort value) => BitConverter.GetBytes(value).Reverse().ToArray();

        public static byte[] Convert(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new byte[] { 0 };
            }

            return Encoding.ASCII
                .GetBytes(value)
                .Concat(new byte[] { 0 }) // end of string \0
                .ToArray();
        }
    }
}