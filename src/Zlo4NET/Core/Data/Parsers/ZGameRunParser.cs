using System.IO;
using System.Text;

using Zlo4NET.Api.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZGameRunParser : IZGameRunParser
    {
        public ZRunResult Parse(ZPacket packet)
        {
            var runResult = ZRunResult.None;

            using (var memoryStream = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                runResult = (ZRunResult) binaryReader.ReadByte();
            }

            return runResult;
        }
    }
}