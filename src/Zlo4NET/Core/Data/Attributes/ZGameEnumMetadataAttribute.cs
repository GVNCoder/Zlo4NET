using System;

namespace Zlo4NET.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    internal class ZGameEnumMetadataAttribute : Attribute
    {
        public string InternalName { get; set; }
    }
}