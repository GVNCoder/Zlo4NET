using System;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Shared;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Services
{
    internal interface IZServerListParser
    {
        Action<ZServerBase, ZServerListAction> ResultCallback { get; set; }

        void Parse(ZPacket[] packets);
        void Close();
    }
}