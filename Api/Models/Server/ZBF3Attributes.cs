using System;
using System.Collections.Generic;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield 3 server attributes
    /// </summary>
    public class ZBF3Attributes : ZAttributesBase
    {
        public ZBF3Attributes(IDictionary<string, string> attributes) : base(attributes)
        { }

        /// <summary>
        /// The Battlefield 3 not supported this attribute.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override string ServerType => throw new NotSupportedException();
        /// <summary>
        /// The Battlefield 3 not supported this attribute.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override string FairFight => throw new NotSupportedException();
        /// <summary>
        /// The Battlefield 3 not supported this attribute.
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        public override string TickRate => throw new NotSupportedException();
    }
}