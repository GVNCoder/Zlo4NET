using System.Collections.Generic;

namespace Zlo4NET.Core.Helpers
{
    internal static class CollectionHelper
    {
        public static IEnumerable<T> GetEmptyEnumerableIfNull<T>(IEnumerable<T> source)
        {
            return source ?? GetEmptyEnumerable<T>();
        }

        public static IEnumerable<T> GetEmptyEnumerable<T>()
        {
            return new T[] { };
        }
    }
}