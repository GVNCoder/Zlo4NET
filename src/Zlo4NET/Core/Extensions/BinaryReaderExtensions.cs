using System.Text;
using System.IO;
using System;

namespace Zlo4NET.Core.Extensions
{
    internal static class BinaryReaderExtensions
    {
        internal static byte[] ReadReversedBytes(this BinaryReader br, int count)
        {
            // get bytes from reader
            var bytes = br.ReadBytes(count);

            // reverse
            Array.Reverse(bytes);

            return bytes;
        }
        internal static string ReadZString(this BinaryReader br)
        {
            var sb = new StringBuilder();

            try
            {
                char t;

                while (br.PeekChar() != -1 && (t = br.ReadChar()) > 0)
                    sb.Append(t);

                return sb.ToString();
            }
            catch (Exception)
            {
                return sb.ToString();
            }
        }

        internal static void SkipZString(this BinaryReader br)
        {
            try
            {
                while (br.PeekChar() != -1 && (br.ReadChar()) > 0)
                    continue;
            }
            catch (Exception)
            {
                // ignore
            }
        }

        internal static void SkipBytes(this BinaryReader br, int bytesNumber)
        {
            br.BaseStream.Seek(bytesNumber, SeekOrigin.Current);
        }
        internal static string ReadCountedString(this BinaryReader br, int count)
        {
            var s = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                s.Append(br.ReadChar());
            }
            return s.ToString();
        }
        internal static uint ReadZUInt32(this BinaryReader br)
        {
            return BitConverter.ToUInt32(br.ReadReversedBytes(4), 0);
        }
        internal static ulong ReadZUInt64(this BinaryReader br)
        {
            return BitConverter.ToUInt64(br.ReadReversedBytes(8), 0);
        }
        internal static ushort ReadZUInt16(this BinaryReader br)
        {
            return BitConverter.ToUInt16(br.ReadReversedBytes(2), 0);
        }
        public static float ReadZFloat(this BinaryReader br)
        {
            return BitConverter.ToSingle(br.ReadReversedBytes(4), 0);
        }
    }
}