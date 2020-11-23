using System.Net;
using System.Net.NetworkInformation;

namespace Zlo4NET.Core.Helpers
{
    /// <summary>
    /// Helps calculate roundtrip delay of the server
    /// </summary>
    public static class ZPingService
    {
        /// <summary>
        /// Calculate roundtrip delay of the <paramref name="address"/>
        /// </summary>
        /// <param name="address">IP address for which the calculation will be executed</param>
        /// <returns>Calculated delay time. If is over 999, then return 999</returns>
        public static int GetPing(IPAddress address)
        {
            var attempts = 2;
            try
            {
                // Try to get the correct ping until all attempts are used
                while (attempts > 0)
                {
                    // send packets to address with timeout 300 ms
                    var pingReply = new Ping().Send(address, 300);

                    if (pingReply?.Status == IPStatus.Success)
                    {
                        return (int) (pingReply.RoundtripTime > 999
                            ? 999
                            : pingReply.RoundtripTime);
                    }

                    attempts--;
                }
            }
            catch
            {
                // ignored
            }

            return 999;
        }
    }
}