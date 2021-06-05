using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZMapNameConverter
    {
        private readonly IDictionary<string, string> _mapsDictionary;

        public ZMapNameConverter(ZGame targetGame)
        {
            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream("maps.json")))
            {
                var content = streamReader.ReadToEnd();
                var jObject = JObject.Parse(content);
                var targetObject = jObject[targetGame.ToString().ToLowerInvariant()];

                _mapsDictionary = targetObject.ToObject<IDictionary<string, string>>();
            }
        }

        public string GetMapNameByKey(string key) => _mapsDictionary.TryGetValue(key, out var value) ? value : key;
    }
}