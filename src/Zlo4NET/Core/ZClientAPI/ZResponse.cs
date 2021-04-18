namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents the unit of response from ZClient
    /// </summary>
    internal class ZResponse
    {
        /// <summary>
        /// Gets or sets response status code
        /// </summary>
        public ZResponseStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets or sets response packets
        /// </summary>
        public ZPacket[] ResponsePackets { get; set; }
        /// <summary>
        /// Gets or sets request instance
        /// </summary>
        public ZRequest Request { get; set; }
    }
}