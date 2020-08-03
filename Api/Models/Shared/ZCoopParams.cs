namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines launch options for a cooperative
    /// </summary>
    public class ZCoopParams
    {
        /// <summary>
        /// Gets or sets play mode. Allow values <see cref="ZPlayMode.CooperativeClient"/> or <see cref="ZPlayMode.CooperativeHost"/>. Required
        /// </summary>
        public ZPlayMode Mode { get; set; }
        /// <summary>
        /// Gets or sets preferred game architecture. Optional
        /// </summary>
        public ZGameArchitecture? PreferredArchitecture { get; set; } = null;
        /// <summary>
        /// Gets or sets level (mission) code. Required for <see cref="ZPlayMode.CooperativeHost"/> play mode
        /// </summary>
        public ZCoopLevels? Level { get; set; } = null;
        /// <summary>
        /// Gets or sets difficulty level. Required for <see cref="ZPlayMode.CooperativeHost"/> play mode
        /// </summary>
        public ZCoopDifficulty? Difficulty { get; set; } = null;
        /// <summary>
        /// Gets or sets cooperative host (friend) ZloID. Required for <see cref="ZPlayMode.CooperativeClient"/> play mode
        /// </summary>
        public uint? FriendId { get; set; } = null;
    }
}