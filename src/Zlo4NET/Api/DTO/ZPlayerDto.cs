using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class ZPlayerDto
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