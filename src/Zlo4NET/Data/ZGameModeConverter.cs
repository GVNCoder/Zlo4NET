using System.Collections.Generic;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Data
{
    internal class ZGameModesConverter
    {
        private readonly IDictionary<string, string> _gameModesDictionary;

        public ZGameModesConverter(ZGame targetGame)
        {
            _gameModesDictionary = ZResource.GetGameModeDictionary(targetGame);
        }

        public string GetGameModeNameByKey(string key) => _gameModesDictionary.TryGetValue(key, out var value) ? value : key;
    }
}