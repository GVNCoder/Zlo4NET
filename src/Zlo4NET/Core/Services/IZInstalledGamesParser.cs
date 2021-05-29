using Zlo4NET.Api.DTOs;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZInstalledGamesParser
    {
        ZInstalledGames Parse(ZPacket packet);
    }
}