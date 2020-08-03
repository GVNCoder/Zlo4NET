namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines launch options for a multiplayer
    /// </summary>
    public class ZMultiParams
    {
        /// <summary>
        /// Gets or sets the target game. Require
        /// </summary>
        public ZGame Game { get; set; } = ZGame.None;
        /// <summary>
        /// Gets or sets server ZloID. Require
        /// </summary>
        public uint ServerId { get; set; }
        /// <summary>
        /// Gets or sets preferred game architecture. Optional
        /// </summary>
        public ZGameArchitecture? PreferredArchitecture { get; set; } = null;
        /// <summary>
        /// Gets or sets player role
        /// </summary>
        public ZRole Role { get; set; } = ZRole.Soldier;
    }
}