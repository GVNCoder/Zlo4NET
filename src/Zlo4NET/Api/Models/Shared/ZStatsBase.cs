namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines base stats
    /// </summary>
    public abstract class ZStatsBase
    {
        /// <summary>
        /// Gets name of rank
        /// </summary>
        public abstract byte Rank { get; }
    }
}