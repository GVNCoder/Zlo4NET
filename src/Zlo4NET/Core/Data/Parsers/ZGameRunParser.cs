using System.IO;
using System.Text;

using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZGameRunParser : IZGameRunParser
    {
        public ZRunResult Parse(ZPacket packet)
        {
            var runStatus = ZRunResult.None;

            using (var memory = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memory, Encoding.ASCII))
            {
                runStatus = (ZRunResult) binaryReader.ReadByte();
            }

            return runStatus;
        }
    }
}