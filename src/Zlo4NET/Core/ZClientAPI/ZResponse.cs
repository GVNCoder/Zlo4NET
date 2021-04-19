namespace Zlo4NET.Core.ZClientAPI
{
    /// <summary>
    /// Represents the unit of response from ZClient
    /// </summary>
    internal class ZResponse
    {
        /// <summary>
        /// Creates instance of <see cref="ZResponse"/>
        /// </summary>
        /// <param name="request">The request</param>
        public ZResponse(ZRequest request)
        {
            Request = request;
        }

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
        public ZRequest Request { get; }
    }
}