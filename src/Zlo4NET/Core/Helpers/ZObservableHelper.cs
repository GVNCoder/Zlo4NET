﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Zlo4NET.Core.Data;
using Zlo4NET.Core.Data.Attributes;

namespace Zlo4NET.Core.Helpers
{
    internal static class ZObservableHelper
    {
        public static IEnumerable<PropertyInfo> GetObservablePropertiesFromType(IReflect reflectType)
            => reflectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propertyInfo =>
                    propertyInfo.GetCustomAttributes(typeof(ZObservablePropertyAttribute), false).Any());

        public static IEnumerable<PropertyInfo> GetObservableObjectPropertiesFromType(IReflect reflectType)
            => reflectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(propertyInfo => propertyInfo.PropertyType.IsSubclassOf(typeof(ZObservableObject)));
    }
}