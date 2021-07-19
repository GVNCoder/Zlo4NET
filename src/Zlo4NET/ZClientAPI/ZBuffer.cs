using System;
using System.Collections.Generic;
using System.Linq;

using Zlo4NET.Helpers;

namespace Zlo4NET.ZClientAPI
{
    internal class ZBuffer
    {
        private byte[] _buffer;

        #region Ctors

        public ZBuffer()
        {
            _buffer = Array.Empty<byte>();
        }
        public ZBuffer(byte[] buffer) : this()
        {
            _buffer = buffer;
        }

        #endregion

        #region Public members

        /// <summary>
        /// Explicit conversion of <see cref="ZBuffer"/> instance into <see cref="byte"/> array buffer
        /// </summary>
        /// <param name="bufferInstance">The <see cref="ZBuffer"/> instance</param>
        /// <returns>Byte array representation of buffer</returns>
        public static implicit operator byte[] (ZBuffer bufferInstance) => bufferInstance?._buffer;
        /// <summary>
        /// Appends to end a byte array buffer
        /// </summary>
        /// <param name="buffer">The byte readonly collection to append</param>
        public void Append(IEnumerable<byte> buffer) => _buffer = _buffer.Concat(buffer).ToArray();
        /// <summary>
        /// Removes specified <paramref name="length"/> (number) of bytes from start of buffer
        /// </summary>
        /// <param name="length"></param>
        public void RemoveBytes(int length) => _buffer = _buffer.Skip(length).ToArray();
        /// <summary>
        /// Sets empty buffer
        /// </summary>
        public void Clear() => _buffer = Array.Empty<byte>();
        /// <summary>
        /// Gets current buffer size
        /// </summary>
        public int Size => _buffer.Length;

        #endregion
    }
}