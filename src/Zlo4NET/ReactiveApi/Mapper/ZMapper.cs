using System.Linq;
using System.Reflection;

namespace Zlo4NET.ReactiveApi.Mapper
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ZMapper<T> where T : ZReactiveObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="source"></param>
        public void MapChanges(T target, T source)
        {
            var mapperProperties = typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.GetCustomAttributes<ZMapperPropertyAttribute>().Any());

            foreach (var property in mapperProperties)
            {
                var sourceValue = property.GetValue(source);
                var targetValue = property.GetValue(target);

                if (sourceValue != null && sourceValue.Equals(targetValue))
                {
                    continue;
                }

                property.SetValue(target, sourceValue);
            }
        }
    }
}