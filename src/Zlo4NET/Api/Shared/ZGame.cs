using Zlo4NET.Core.Data.Attributes;

// ReSharper disable InconsistentNaming

namespace Zlo4NET.Api.Shared
{
    /// <summary>
    /// Defines supported games
    /// </summary>
    public enum ZGame
    {
        /// <summary>
        /// Battlefield 3
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BF3", DefaultArchitecture = ZGameArchitecture.x32, GameReference = BF3)]
        BF3,
        /// <summary>
        /// Battlefield 4
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BF4", GameReference = BF4)]
        BF4,
        /// <summary>
        /// Battlefield Hardline
        /// </summary>
        [ZGameEnumMetadata(InternalName = "Z.BFHL", DefaultArchitecture = ZGameArchitecture.x64, GameReference = BFHL)]
        BFHL,

        //[ZGameEnumMetadata(InternalName = "Z.MOHW")]
        //MOHW

        /// <summary>
        /// Stub
        /// </summary>
        None = 255
    }
}