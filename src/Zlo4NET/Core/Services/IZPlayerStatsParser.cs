using Zlo4NET.Api.DTO;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZPlayerStatsParser
    {
        ZPlayerStatsDto Parse(ZPacket packet);
    }
}