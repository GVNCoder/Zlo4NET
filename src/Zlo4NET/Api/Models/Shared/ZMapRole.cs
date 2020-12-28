namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Rotation role
    /// </summary>
    public enum ZMapRole : byte
    {
        /// <summary>
        /// Map in map list rotation
        /// </summary>
        Other,
        /// <summary>
        /// Current map
        /// </summary>
        Current,
        /// <summary>
        /// Next map
        /// </summary>
        Next
    }
}