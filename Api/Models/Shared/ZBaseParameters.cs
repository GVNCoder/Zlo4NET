namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines basic launch options
    /// </summary>
    public abstract class ZBaseParameters
    {
        /// <summary>
        /// Gets or sets the target game. Require
        /// </summary>
        public virtual ZGame Game { get; set; } = ZGame.None;
        /// <summary>
        /// Gets or sets preferred game architecture. Optional
        /// </summary>
        public virtual ZGameArchitecture? PreferredArchitecture { get; set; } = null;
    }
}