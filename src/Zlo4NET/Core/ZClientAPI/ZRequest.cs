using System.Linq;

using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.ZClient.Data;

namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents the unit of request to ZClient
    /// </summary>
    internal class ZRequest
    {
        /// <summary>
        /// Gets or sets request command
        /// </summary>
        public ZCommand RequestCommand { get; set; }
        /// <summary>
        /// Gets or sets request payload
        /// </summary>
        public byte[] RequestPayload { get; set; }
        /// <summary>
        /// Converts this instance to byte array representation
        /// </summary>
        /// <returns>Request in byte array representation</returns>
        public byte[] ToByteArray()
        {
            var requestBytes = new[] { (byte) RequestCommand }
                .Concat(ZBitConverter.Convert(RequestPayload.Length))
                .Concat(RequestPayload)
                .ToArray();

            return requestBytes;
        }
    }
}