using System;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class ZGameEnumMetadataAttribute : Attribute
    {
        public string InternalName { get; set; }
        public ZGameArchitecture DefaultArchitecture { get; set; }
        public ZGame GameReference { get; set; }
    }
}