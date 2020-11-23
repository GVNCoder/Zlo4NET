using System.Text;
using System.IO;
using System;

namespace Zlo4NET.Core.Extensions
{
    /// <summary>
    /// BinaryReader extensions
    /// </summary>
    internal static class BinaryReaderExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static byte[] ReadReversedBytes(this BinaryReader br, int count)
        {
            // get bytes from reader
            var bytes = br.ReadBytes(count);

            // reverse
            Array.Reverse(bytes);

            return bytes;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        internal static string ReadZString(this BinaryReader br)
        {
            // create sb
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        internal static string ReadCountedString(this BinaryReader br, int count)
        {
            var s = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                s.Append(br.ReadChar());
            }
            return s.ToString();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        internal static uint ReadZUInt32(this BinaryReader br)
        {
            return BitConverter.ToUInt32(br.ReadReversedBytes(4), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        internal static ulong ReadZUInt64(this BinaryReader br)
        {
            return BitConverter.ToUInt64(br.ReadReversedBytes(8), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        internal static ushort ReadZUInt16(this BinaryReader br)
        {
            return BitConverter.ToUInt16(br.ReadReversedBytes(2), 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="br"></param>
        /// <returns></returns>
        public static float ReadZFloat(this BinaryReader br)
        {
            return BitConverter.ToSingle(br.ReadReversedBytes(4), 0);
        }
    }
}