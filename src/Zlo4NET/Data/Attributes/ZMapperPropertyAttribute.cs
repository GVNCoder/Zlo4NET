using System;

namespace Zlo4NET.Data.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Indicates mapper property (observable property)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ZMapperPropertyAttribute : Attribute
    {
        
    }
}