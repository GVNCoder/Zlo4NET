using System.Collections.Generic;

using Zlo4NET.Api.Shared;

namespace Zlo4NET.Data
{
    internal class ZMapNameConverter
    {
        private readonly IDictionary<string, string> _mapsDictionary;

        public ZMapNameConverter(ZGame targetGame)
        {
            _mapsDictionary = ZResource.GetGameMapDictionary(targetGame);
        }

        public string GetMapNameByKey(string key) => _mapsDictionary.TryGetValue(key, out var value) ? value : key;
    }
}