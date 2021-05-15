using System;

namespace Zlo4NET.Core.Data.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    internal class ZStatsMapperAttribute : Attribute
    {
        public string MapFromDictionaryKey { get; set; }
    }
}