using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Api.DTO
{
    /// <summary>
    /// 
    /// </summary>
    public class ZMapDto
    {
        /// <summary>
        /// Gets name of map
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets name of game mode
        /// </summary>
        public string GameModeName { get; set; }
        /// <summary>
        /// Gets position in rotation
        /// </summary>
        public ZMapInMapRotation InRotationPosition { get; set; }
        /// <summary>
        /// Gets raw map data keys
        /// </summary>
        public string[] RawKeys { get; set; }
    }
}