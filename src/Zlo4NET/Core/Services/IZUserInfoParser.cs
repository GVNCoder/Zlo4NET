using Zlo4NET.Api.DTOs;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
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