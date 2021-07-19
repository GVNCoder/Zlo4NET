using Zlo4NET.Api.DTOs;
using Zlo4NET.ZClientAPI;

namespace Zlo4NET.Services
{
    internal interface IZUserInfoParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>Parsed data</returns>
        ZUser Parse(ZPacket packet);
    }
}