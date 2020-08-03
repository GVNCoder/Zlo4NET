using System.IO;
using System.Linq;
using System.Text;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZUserInfoParser : IZUserInfoParser
    {
        public ZUser Parse(ZPacket[] packets)
        {
            var packet = packets
                .First();

            var user = new ZUser();

            using (var memory = new MemoryStream(packet.Content, false))
            using (var br = new BinaryReader(memory, Encoding.ASCII))
            {
                user.Id   = br.ReadZUInt32();
                user.Name = br.ReadZString();
            }

            return user;
        }
    }
}