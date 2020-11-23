using System;

namespace Zlo4NET.Core.Data.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Indicates changeable property (observable property)
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class ZObservablePropertyAttribute : Attribute
    {

    }
}