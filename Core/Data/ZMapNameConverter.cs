using System;
using System.IO;
using Newtonsoft.Json.Linq;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.Data
{
    internal class ZMapNameConverter
    {
        private readonly JToken _jBF3MapDictionary;
        private readonly JToken _jBF4MapDictionary;
        private readonly JToken _jBFHMapDictionary;

        private JToken _GetJToken(ZGame game)
        {
            switch (game)
            {
                case ZGame.BF3: return _jBF3MapDictionary;
                case ZGame.BF4: return _jBF4MapDictionary;
                case ZGame.BFH: return _jBFHMapDictionary;

                case ZGame.None:
                default: throw new Exception();
            }
        }

        public ZMapNameConverter()
        {
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream("maps.json")))
            {
                var content = sr.ReadToEnd();
                var jObject = JObject.Parse(content);

                _jBF3MapDictionary = jObject[ZStringConstants.BF3ResourceKey];
                _jBF4MapDictionary = jObject[ZStringConstants.BF4ResourceKey];
                _jBFHMapDictionary = jObject[ZStringConstants.BFHResourceKey];
            }
        }

        public string GetMapNameByKey(ZGame game, string mapNameKey)
        {
            try
            {
                return _GetJToken(game)[mapNameKey].ToObject<string>();
            }
            catch (Exception)
            {
                return ZStringConstants.NotAvailable;
            }
        }
    }
}