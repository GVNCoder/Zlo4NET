using System.Text;
using System.IO;
using System;
using System.Linq;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Core.Extensions
{
    internal static class ZBinaryReaderExtensions
    {
        #region Private helpers
        private static byte[] ReadReversedBytes(this BinaryReader binaryReader, int count)
        {
            var bytes = binaryReader.ReadBytes(count)
                .Reverse()
                .ToArray();
            
            return bytes;
        }

        #endregion

        public static long BytesRemaining(this BinaryReader binaryReader) => binaryReader.BaseStream.Length - binaryReader.BaseStream.Position;
        public static string ReadZString(this BinaryReader binaryReader)
        {
            var stringBuilder = new StringBuilder();

            try
            {
                char t;
                while (binaryReader.PeekChar() != -1 && (t = binaryReader.ReadChar()) > 0)
                {
                    stringBuilder.Append(t);
                }
            }
            catch (Exception)
            {
                // ignore
            }

            return stringBuilder.ToString();
        }
        public static void SkipZString(this BinaryReader binaryReader)
        {
            try
            {
                while (binaryReader.PeekChar() != -1 && (binaryReader.ReadChar()) > 0)
                {
                    // ...
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }
        public static void SkipBytes(this BinaryReader binaryReader, int numberOfBytes) => binaryReader.BaseStream.Seek(numberOfBytes, SeekOrigin.Current);
        public static string ReadCountedString(this BinaryReader binaryReader, int count)
        {
            var stringBuilder = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                stringBuilder.Append(binaryReader.ReadChar());
            }

            return stringBuilder.ToString();
        }
        public static uint ReadZUInt32(this BinaryReader binaryReader) => BitConverter.ToUInt32(binaryReader.ReadReversedBytes(sizeof(uint)), 0);
        public static ulong ReadZUInt64(this BinaryReader binaryReader) => BitConverter.ToUInt64(binaryReader.ReadReversedBytes(sizeof(ulong)), 0);
        public static ushort ReadZUInt16(this BinaryReader binaryReader) => BitConverter.ToUInt16(binaryReader.ReadReversedBytes(sizeof(ushort)), 0);
        public static float ReadZFloat(this BinaryReader binaryReader) => BitConverter.ToSingle(binaryReader.ReadReversedBytes(sizeof(float)), 0);
    }
}