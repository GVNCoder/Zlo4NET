using System.IO;
using System.Linq;
using System.Text;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZGameRunParser : IZGameRunParser
    {
        public ZRunResult Parse(ZPacket[] packets)
        {
            var packet = packets
                .First();

            var runStatus = ZRunResult.None;

            using (var memory = new MemoryStream(packet.Content, false))
            using (var br = new BinaryReader(memory, Encoding.ASCII))
            {
                runStatus = (ZRunResult) br.ReadByte();
            }

            return runStatus;
        }
    }
}