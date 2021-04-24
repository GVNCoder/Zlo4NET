using Zlo4NET.Api.DTO;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    internal interface IZUserInfoParser
    {
        ZUserDTO Parse(ZPacket packet);
    }
}