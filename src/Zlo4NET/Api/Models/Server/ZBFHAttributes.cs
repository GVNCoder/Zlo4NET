using System.Collections.Generic;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Api.Models.Server
{
    /// <summary>
    /// Defines the Battlefield Hardline server attributes
    /// </summary>
    public class ZBFHAttributes : ZAttributesBase
    {
        public ZBFHAttributes(IDictionary<string, string> attributes) : base(attributes)
        { }

        /// <inheritdoc />
        [ZObservableProperty]
        public override string FairFight => _getValue("fairfight");
        /// <inheritdoc />
        public override string TickRate => _getValue("tickrate");
    }
}