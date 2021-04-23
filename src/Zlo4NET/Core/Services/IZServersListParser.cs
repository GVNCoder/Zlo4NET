using System;

using Zlo4NET.Api.Models.Server;
using Zlo4NET.Core.Data;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZServersListParser
    {
        Action<ZServerBase, ZServerParserAction> ResultCallback { get; set; }
        void ParseAsync(ZPacket[] packets);
        void Close();
    }
}