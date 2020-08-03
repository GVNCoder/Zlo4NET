using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.Data
{
    internal class ZGameModesConverter
    {
        private readonly JToken _jBF3GameModeDictionary;
        private readonly JToken _jBF4GameModeDictionary;
        private readonly JToken _jBFHGameModeDictionary;

        private JToken _GetJToken(ZGame game)
        {
            switch (game)
            {
                case ZGame.BF3: return _jBF3GameModeDictionary;
                case ZGame.BF4: return _jBF4GameModeDictionary;
                case ZGame.BFH: return _jBFHGameModeDictionary;

                case ZGame.None:
                default: throw new Exception();
            }
        }

        public ZGameModesConverter()
        {
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream("gameModes.json")))
            {
                var content = sr.ReadToEnd();
                var jObject = JObject.Parse(content);

                _jBF3GameModeDictionary = jObject[ZStringConstants.BF3ResourceKey];
                _jBF4GameModeDictionary = jObject[ZStringConstants.BF4ResourceKey];
                _jBFHGameModeDictionary = jObject[ZStringConstants.BFHResourceKey];
            }
        }

        public string GetGameModeNameByKey(ZGame game, string gameModeKey)
        {
            try
            {
                return _GetJToken(game)[gameModeKey].ToObject<string>();
            }
            catch (Exception)
            {
                return ZStringConstants.NotAvailable;
            }
        }
    }
}