namespace Zlo4NET.ZClientAPI
{
    /// <summary>
    /// Represents the unit of response from ZClient
    /// </summary>
    internal class ZResponse
    {
        public ZResponse(ZRequest request)
        {
            Request = request;
        }

        /// <summary>
        /// Gets or sets status code of response
        /// </summary>
        public ZResponseStatusCode StatusCode { get; set; }
        /// <summary>
        /// Gets or sets response packets
        /// </summary>
        public ZPacket[] ResponsePackets { get; set; }
        /// <summary>
        /// Gets or sets related request
        /// </summary>
        public ZRequest Request { get; }
    }
}