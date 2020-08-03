namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines launch options for a test range (playground)
    /// </summary>
    public class ZTestRangeParams
    {
        /// <summary>
        /// Gets or sets the target game. Require
        /// </summary>
        public ZGame Game { get; set; } = ZGame.None;
        /// <summary>
        /// Gets or sets preferred game architecture. Optional
        /// </summary>
        public ZGameArchitecture? PreferredArchitecture { get; set; } = null;
    }
}