using System.Net;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace Zlo4NET.Core.Helpers
{
    /// <summary>
    /// Helps calculate roundtrip delay of the server
    /// </summary>
    public static class ZPingHelper
    {
        /// <summary>
        /// Calculate roundtrip delay of the <paramref name="host"/>
        /// </summary>
        /// <param name="host">IP address for which the calculation will be executed</param>
        /// <param name="attemptsCount"></param>
        /// <param name="defaultValue"></param>
        /// <param name="timeout"></param>
        /// <returns>Calculated delay time</returns>
        public static async Task<long> GetPingAsync(IPAddress host, uint attemptsCount = 1, long defaultValue = -1, int timeout = 350)
        {
            var ping = new Ping();

            try
            {
                // try to get the ping until all attempts are used
                while (attemptsCount > 0)
                {
                    // send ping request to host
                    var pingReply = await ping.SendPingAsync(host, timeout);
                    if (pingReply.Status == IPStatus.Success)
                    {
                        return pingReply.RoundtripTime;
                    }

                    // we are used one attempt
                    attemptsCount--;
                }
            }
            catch
            {
                // ignore
            }

            return defaultValue;
        }
    }
}