using System.Collections.Generic;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield 4 server attributes
    /// </summary>
    public class ZBF4Attributes : ZAttributesBase
    {
        public ZBF4Attributes(IDictionary<string, string> attributes) : base(attributes)
        { }

        /// <inheritdoc />
        public override string ServerType => _getValue("servertype");
        /// <inheritdoc />
        [ZObservableProperty]
        public override string FairFight => _getValue("fairfight");
        /// <inheritdoc />
        public override string TickRate => _getValue("tickrate");
    }
}