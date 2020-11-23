using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.Services
{
    /// <summary>
    /// Defines user info parser
    /// </summary>
    internal interface IZUserInfoParser
    {
        /// <summary>
        /// Parses provided packets
        /// </summary>
        /// <param name="packets">The packets for parsing</param>
        /// <returns>Parsed data</returns>
        ZUser Parse(ZPacket[] packets);
    }
}