using Zlo4NET.Core.Data.Attributes;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Api.Models.Shared
{
    /// <summary>
    /// Defines supported games
    /// </summary>
    public enum ZGame
    {
        /// <summary>
        /// Battlefield 3
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BF3")]
        BF3,
        /// <summary>
        /// Battlefield 4
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BF4")]
        BF4,
        /// <summary>
        /// Battlefield Hardline
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BFHL")]
        BFHL,

        //[ZGameEnumMetadata(InternalName = "Z.MOHW")]
        //MOHW

        /// <summary>
        /// Stub
        /// </summary>
        None = 255
    }
}