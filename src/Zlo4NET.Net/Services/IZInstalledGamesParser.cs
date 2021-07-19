using Zlo4NET.Api.DTOs;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Services
{
    internal interface IZInstalledGamesParser
    {
        ZInstalledGamesCollection Parse(ZPacket packet);
    }
}