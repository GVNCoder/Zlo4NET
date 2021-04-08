﻿using System.Collections.Generic;
using System.Linq;

using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents the buffer based on byte array with additional helping methods
    /// </summary>
    public class ZBuffer
    {
        private byte[] _buffer;

        #region Ctors

        /// <summary>
        /// Creates empty buffer
        /// </summary>
        public ZBuffer()
        {
            _buffer = CollectionHelper.GetEmptyEnumerable<byte>()
                .ToArray();
        }
        /// <inheritdoc />
        /// <summary>
        /// Creates buffer from <paramref name="buffer" />
        /// </summary>
        /// <param name="buffer">The buffer</param>
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
        public static explicit operator byte[] (ZBuffer bufferInstance) => bufferInstance?._buffer;
        /// <summary>
        /// Appends to end a byte array buffer
        /// </summary>
        /// <param name="buffer">The byte readonly collection to append</param>
        public void Append(IReadOnlyList<byte> buffer) => _buffer = _buffer.Concat(buffer).ToArray();
        /// <summary>
        /// Removes specified <paramref name="length"/> (number) of bytes from start of buffer
        /// </summary>
        /// <param name="length"></param>
        public void RemoveBytes(int length) => _buffer = _buffer.Skip(length).ToArray();
        /// <summary>
        /// Sets empty buffer
        /// </summary>
        public void Clear() => _buffer = CollectionHelper.GetEmptyEnumerable<byte>().ToArray();

        #endregion
    }
}