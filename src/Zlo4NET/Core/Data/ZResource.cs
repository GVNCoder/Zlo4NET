using System.Collections.Generic;
using System.IO;

using Newtonsoft.Json.Linq;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Core.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class ZResource
    {
        #region Public interface

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetGame"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetGameMapDictionary(ZGame targetGame)
        {
            var key = targetGame.ToString().ToLowerInvariant();
            var jObject = _LoadResourceByKey("gamemap");
            var targetSubObject = jObject[key];

            return targetSubObject.ToObject<IDictionary<string, string>>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetGame"></param>
        /// <returns></returns>
        public static IDictionary<string, string> GetGameModeDictionary(ZGame targetGame)
        {
            var key = targetGame.ToString().ToLowerInvariant();
            var jObject = _LoadResourceByKey("gamemode");
            var targetSubObject = jObject[key];

            return targetSubObject.ToObject<IDictionary<string, string>>();
        }

        #endregion

        #region Private helpers

        private static JObject _LoadResourceByKey(string key)
        {
            using (var streamReader = new StreamReader(ZInternalResource.GetResourceStream($"shared.{key}.json")))
            {
                var content = streamReader.ReadToEnd();
                return JObject.Parse(content);
            }
        }

        #endregion
    }
}