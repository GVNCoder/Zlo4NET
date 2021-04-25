using System.Net;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZUIntToIpAddress
    {
        public static IPAddress Convert(uint ipAddress)
        {
            return new IPAddress(ZBitConverter.Convert(ipAddress));
        }
    }
}