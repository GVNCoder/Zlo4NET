using System.Collections.Generic;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;

namespace Zlo4NET.Core.Data
{
    internal class ZChangesMapper : IZChangesMapper
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