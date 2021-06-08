using System.Collections.Generic;
using Zlo4NET.Helpers;

namespace Zlo4NET.Data
{
    internal class ZChangesMapper
    {
        public void MapChanges<T>(T source, T target)
        {
            var observableProperties = ZMapperHelper.GetMapperPropertiesFromType(typeof(T));

            foreach (var observableProperty in observableProperties)
            {
                var sourceValue = observableProperty.GetValue(source);
                var targetValue = observableProperty.GetValue(target);

                if (sourceValue != null && sourceValue.Equals(targetValue))
                {
                    continue;
                }

                observableProperty.SetValue(target, sourceValue);
            }
        }

        public void MapCollection<T>(IEnumerable<T> source, ICollection<T> target)
        {
            target.Clear();

            foreach (var item in source)
            {
                target.Add(item);
            }
        }
    }
}