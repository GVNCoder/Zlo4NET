using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZGameModesConverter
    {
        private readonly IDictionary<string, string> _gameModesDictionary;

        public ZGameModesConverter(ZGame targetGame)
        {
            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream("gameModes.json")))
            {
                var content = streamReader.ReadToEnd();
                var jObject = JObject.Parse(content);
                var targetObject = jObject[targetGame.ToString().ToLowerInvariant()];

                _gameModesDictionary = targetObject.ToObject<IDictionary<string, string>>();
            }
        }

        public string GetGameModeNameByKey(string key) => _gameModesDictionary.TryGetValue(key, out var value) ? value : key;
    }
}