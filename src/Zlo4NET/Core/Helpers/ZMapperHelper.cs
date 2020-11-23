using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZMapperHelper
    {
        public static IEnumerable<PropertyInfo> GetMapperPropertiesFromType(IReflect reflectType)
            => reflectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propertyInfo =>
                    propertyInfo.GetCustomAttributes(typeof(ZMapperPropertyAttribute), false).Any());
    }
}