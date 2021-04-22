using System.IO;
using System.Text;

using Zlo4NET.Api.DTO;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZUserInfoParser : IZUserInfoParser
    {
        public ZUserDTO Parse(ZPacket packet)
        {
            var user = new ZUserDTO();

            using (var memory = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memory, Encoding.ASCII))
            {
                user.UserId   = binaryReader.ReadZUInt32();
                user.UserName = binaryReader.ReadZString();
            }

            return user;
        }
    }
}