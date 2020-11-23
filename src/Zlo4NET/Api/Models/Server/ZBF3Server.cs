using System;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield 3 server
    /// </summary>
    public class ZBF3Server : ZServerBase
    {
        /// <summary>
        /// This property not supported on Battlefield 3
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override byte SpectatorsCapacity { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    }
}