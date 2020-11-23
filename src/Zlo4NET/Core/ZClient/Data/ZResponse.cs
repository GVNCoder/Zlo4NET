namespace Zlo4NET.Core.ZClient.Data
{
    /// <summary>
    /// Defines the ZClient response
    /// </summary>
    internal class ZResponse
    {
        public ZCommand Id { get; set; }
        public ZPacket[] Packets { get; set; }
        public ZResponseStatusCode Status { get; set; } = ZResponseStatusCode.None;
        public ZRequest Request { get; set; }
    }
}