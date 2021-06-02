using System;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZServersListParser
    {
        Action<ZServer, ZServerListAction> OnParsingResultCallback { get; set; }
        void Parse(ZPacket[] packets);
        void Close();
    }
}