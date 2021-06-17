using System.IO;
using System.Text;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Extensions;
using Zlo4NET.Services;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Data.Parsers
{
    internal class ZUserInfoParser : IZUserInfoParser
    {
        public ZUser Parse(ZPacket packet)
        {
            var user = new ZUser();

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