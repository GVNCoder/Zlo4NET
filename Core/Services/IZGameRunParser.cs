using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Services
{
    internal interface IZGameRunParser
    {
        ZRunResult Parse(ZPacket[] packets);
    }
}