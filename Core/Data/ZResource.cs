using System.IO;
using Newtonsoft.Json.Linq;
using Zlo4NET.Core.Helpers;

namespace Zlo4NET.Core.Data
{
    /// <summary>
    /// Provides some resources for general use
    /// </summary>
    public static class ZResource
    {
        private static JToken _getResourceObject(string key, string resource)
        {
            JObject jObject = null;
            using (var sr = new StreamReader(ZInternalResource.GetResourceStream($"shared.{resource}.json")))
            {
                var content = sr.ReadToEnd();
                jObject = JObject.Parse(content);
            }

            return jObject[key];
        }

        /// <summary>
        /// Gets Battlefield 3 maps array
        /// </summary>
        /// <returns>String array of map names</returns>
        public static string[] GetBF3MapNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BF3ResourceKey, "map");
            return jObject.ToObject<string[]>();
        }
        /// <summary>
        /// Gets Battlefield 4 maps array
        /// </summary>
        /// <returns>String array of map names</returns>
        public static string[] GetBF4MapNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BF4ResourceKey, "map");
            return jObject.ToObject<string[]>();
        }
        /// <summary>
        /// Gets Battlefield Hardline maps array
        /// </summary>
        /// <returns>String array of map names</returns>
        public static string[] GetBFHMapNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BFHResourceKey, "map");
            return jObject.ToObject<string[]>();
        }
        /// <summary>
        /// Gets Battlefield 3 game modes array
        /// </summary>
        /// <returns>String array of game mode names</returns>
        public static string[] GetBF3GameModeNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BF3ResourceKey, "gamemode");
            return jObject.ToObject<string[]>();
        }
        /// <summary>
        /// Gets Battlefield 4 game modes array
        /// </summary>
        /// <returns>String array of game mode names</returns>
        public static string[] GetBF4GameModeNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BF4ResourceKey, "gamemode");
            return jObject.ToObject<string[]>();
        }
        /// <summary>
        /// Gets Battlefield Hardline game modes array
        /// </summary>
        /// <returns>String array of game mode names</returns>
        public static string[] GetBFHGameModeNames()
        {
            var jObject = _getResourceObject(ZStringConstants.BFHResourceKey, "gamemode");
            return jObject.ToObject<string[]>();
        }
    }
}