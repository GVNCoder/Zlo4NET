using System;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZServerListParser
    {
        Action<ZServerBase, ZServerListAction> ResultCallback { get; set; }

        void Parse(ZPacket[] packets);
        void Close();
    }
}