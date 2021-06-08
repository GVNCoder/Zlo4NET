using Zlo4NET.Api.Shared;

// ReSharper disable ClassNeverInstantiated.Global

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZPlayer
    {
        /// <summary>
        /// Gets player slot number
        /// </summary>
        public byte Slot { get; set; }
        /// <summary>
        /// Gets player identifier
        /// </summary>
        public uint Id { get; set; }
        /// <summary>
        /// Gets player name
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets player role
        /// </summary>
        public ZPlayerRole Role { get; set; }
    }
}