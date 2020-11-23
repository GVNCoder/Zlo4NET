using System.Collections.Generic;
using System.Linq;

using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.Data
{
    internal class _Buffer
    {
        private byte[] _buffer;

        public _Buffer()
        {
            _buffer = CollectionHelper.GetEmptyEnumerable<byte>()
                .ToArray();
        }

        public _Buffer(byte[] bufferData) : this()
        {
            _buffer = bufferData;
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once ConvertToAutoPropertyWhenPossible
        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public byte[] BufferData => _buffer;
        public int Size => _buffer.Length;

        public void Append(IEnumerable<byte> bufferData) => _buffer = _buffer.Concat(bufferData).ToArray();
        public void RemoveBytes(int numOfBytes) => _buffer = _buffer.Skip(numOfBytes).ToArray();
        public void Clear() => _buffer = CollectionHelper.GetEmptyEnumerable<byte>().ToArray();
    }
}