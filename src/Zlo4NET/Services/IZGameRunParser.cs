using Zlo4NET.Api.Shared;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Services
{
    internal interface IZGameRunParser
    {
        ZRunResult Parse(ZPacket packet);
    }
}