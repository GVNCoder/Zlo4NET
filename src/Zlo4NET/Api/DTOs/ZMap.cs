using Zlo4NET.Api.Shared;

// ReSharper disable ClassNeverInstantiated.Global

namespace Zlo4NET.Api.DTOs
{
    /// <summary>
    /// 
    /// </summary>
    public class ZMap
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