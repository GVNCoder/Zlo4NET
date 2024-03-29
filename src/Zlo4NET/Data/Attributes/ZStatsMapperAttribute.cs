using System;

namespace Zlo4NET.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class ZStatsMapperAttribute : Attribute
    {
        public string MapFromDictionaryKey { get; set; }
        public Type TargetType { get; set; }
    }
}