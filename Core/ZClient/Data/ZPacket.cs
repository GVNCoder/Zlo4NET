namespace Zlo4NET.Core.ZClient.Data
{
    /// <summary>
    /// Defines response packet
    /// </summary>
    internal struct ZPacket
    {
        /// <summary>
        /// The packet id
        /// </summary>
        public ZCommand Id { get; set; }
        /// <summary>
        /// The content bytes
        /// </summary>
        public byte[] Content { get; set; }
        /// <summary>
        /// Packet length
        /// </summary>
        public int Length { get; set; }
    }
}