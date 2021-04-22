using Zlo4NET.Api.DTO;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Services
{
    /// <summary>
    /// 
    /// </summary>
    internal interface IZUserInfoParser
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns>Parsed data</returns>
        ZUserDTO Parse(ZPacket packet);
    }
}