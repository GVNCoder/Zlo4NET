// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming

using Zlo4NET.Data.Attributes;

namespace Zlo4NET.Api.Shared
{
    /// <summary>
    /// Defines CoOp level codes
    /// </summary>
    public enum ZCoopLevels
    {
        /// <summary>
        /// Operation Exodus
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_007")]
        COOP_007,
        /// <summary>
        /// Fire from the Sky
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_006")]
        COOP_006,
        /// <summary>
        /// Exfiltration
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_009")]
        COOP_009,
        /// <summary>
        /// Hit and Run
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_002")]
        COOP_002,
        /// <summary>
        /// Drop 'Em Like Liquid
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_003")]
        COOP_003,
        /// <summary>
        /// The Eleventh Hour
        /// </summary>
        [ZCoopLevelEnumMetadata(InternalName = "COOP_010")]
        COOP_010
    }
}