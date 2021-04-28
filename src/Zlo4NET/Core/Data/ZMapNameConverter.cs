using System.IO;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZMapNameConverter
    {
        private readonly JToken _jMapDictionary;

        public ZMapNameConverter(ZGame targetGame)
        {
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream("maps.json")))
            {
                var content = sr.ReadToEnd();
                var jObject = JObject.Parse(content);

                _jMapDictionary = jObject[targetGame.ToString().ToLowerInvariant()];
            }
        }

        public string GetMapNameByKey(string mapNameKey) => _jMapDictionary[mapNameKey]?.ToObject<string>();
    }
}