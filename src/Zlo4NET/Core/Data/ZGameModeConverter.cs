using System.IO;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Models.Shared;

namespace Zlo4NET.Core.Data
{
    internal class ZGameModesConverter
    {
        private readonly JToken _jGameModeDictionary;

        public ZGameModesConverter(ZGame targetGame)
        {
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream("gameModes.json")))
            {
                var content = sr.ReadToEnd();
                var jObject = JObject.Parse(content);

                _jGameModeDictionary = jObject[targetGame.ToString().ToLowerInvariant()];
            }
        }

        public string GetGameModeNameByKey(string gameModeKey) => _jGameModeDictionary[gameModeKey].ToObject<string>();
    }
}