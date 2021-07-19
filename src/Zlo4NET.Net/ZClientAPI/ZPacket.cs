namespace Zlo4NET.ZClientAPI
{
    /// <summary>
    /// Represents the unit of communication with the ZClient
    /// </summary>
    internal struct ZPacket
    {
        /// <summary>
        /// The packet identifier
        /// </summary>
        public ZCommand Id { get; set; }
        /// <summary>
        /// The packet length
        /// </summary>
        public int Length => Payload?.Length ?? 0;
        /// <summary>
        /// The payload
        /// </summary>
        public byte[] Payload { get; set; }
    }
}