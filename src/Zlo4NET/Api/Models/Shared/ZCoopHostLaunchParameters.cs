// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Zlo4NET.Api.Models.Shared
{
    /// <inheritdoc />
    public class ZCoopHostLaunchParameters : ZBaseLaunchParameters
    {
        /// <summary>
        /// 
        /// </summary>
        public ZCoopLevels? Level { get; set; } = null;
        /// <summary>
        /// 
        /// </summary>
        public ZCoopDifficulty? Difficulty { get; set; } = null;

    }
}