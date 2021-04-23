using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZStatsParser
    {
        ZBF3Stats ParseBF3Stats(ZPacket[] packets);
        ZBF4Stats ParseBF4Stats(ZPacket[] packets);
    }
}