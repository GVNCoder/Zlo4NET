using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines player
    /// </summary>
    public class ZPlayer
    {
        /// <summary>
        /// Gets player slot number
        /// </summary>
        public byte Slot { get; set; }
        /// <summary>
        /// Gets player ZloID
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Gets name of player
        /// </summary>
        public string Name { get; set; }

        public ZPlayerRole Role { get; set; }
    }
}