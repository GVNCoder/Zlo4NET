using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZGameRunParser
    {
        ZRunResult Parse(ZPacket packet);
    }
}