using System.Linq;

namespace Zlo4NET.Core.Client
{
    internal class ZBuffer
    {
        public byte[] InternalBuffer { get; private set; } = { };
        public int Size => InternalBuffer.Length;

        public void Clear() => InternalBuffer = new byte[] { };
        public void Append(byte[] bytes) => InternalBuffer = InternalBuffer.Concat(bytes).ToArray();
    }
}