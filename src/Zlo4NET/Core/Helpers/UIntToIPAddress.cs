using System.Net;

namespace Zlo4NET.Core.Helpers
{
    /// <summary>
    /// Convertor <see cref="uint"/> to <see cref="IPAddress"/>
    /// </summary>
    internal static class UIntToIPAddress
    {
        internal static IPAddress Convert(uint ipAddress)
        {
            return new IPAddress(
                ZBitConverter.Convert(ipAddress));
        }
    }
}