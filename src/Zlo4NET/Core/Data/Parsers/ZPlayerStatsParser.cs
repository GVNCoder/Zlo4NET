using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Zlo4NET.Api.DTOs;
using Zlo4NET.Api.Models.Shared;
using Zlo4NET.Core.Extensions;
using Zlo4NET.Core.Helpers;
using Zlo4NET.Core.Services;
using Zlo4NET.Core.ZClientAPI;

namespace Zlo4NET.Core.Data.Parsers
{
    internal class ZPlayerStatsParser : IZPlayerStatsParser
    {
        #region Internal types

        // ReSharper disable once InconsistentNaming
        private class _Stats
        {
            public IDictionary<string, float> Stats;
            public ZGame TargetGame;
        }

        #endregion

        private readonly ZLogger _logger;

        private readonly IDictionary<ZGame, Func<IDictionary<string, float>, ZPlayerStatsBase>>
            _gameSpecificStatsHandlers = new Dictionary<ZGame, Func<IDictionary<string, float>, ZPlayerStatsBase>>
            {
                { ZGame.BF3, ZGameSpecificStatsHandlerProvider.BF3StatsHandler },
                { ZGame.BF4, ZGameSpecificStatsHandlerProvider.BF4StatsHandler },
            };

        #region Ctor

        public ZPlayerStatsParser()
        {
            _logger = ZLogger.Instance;
        }

        #endregion

        #region IZPlayerStatsParser interface

        public ZPlayerStatsBase Parse(ZPacket packet)
        {
            // parse stats object and load some internal resources
            var statsObject = _ParseStatsObject(packet);

            // assign stats to object
            var statsAssignHandler = _gameSpecificStatsHandlers[statsObject.TargetGame];
            var stats = statsAssignHandler.Invoke(statsObject.Stats);

            return stats;
        }

        #endregion

        #region Private helpers

        private static _Stats _ParseStatsObject(ZPacket packet)
        {
            _Stats stats = null;

            using (var memoryStream = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                // parse packet payload
                var targetGame = (ZGame) binaryReader.ReadByte();
                var countOfStats = binaryReader.ReadZUInt16();
                var statsDictionary = new Dictionary<string, float>(countOfStats);

                for (ushort i = 0; i < countOfStats; i++)
                {
                    var name = binaryReader.ReadZString();
                    var value = binaryReader.ReadZFloat();

                    statsDictionary.Add(name, value);
                }

                stats = new _Stats
                {
                    Stats = statsDictionary,
                    TargetGame = targetGame
                };
            }

            return stats;
        }

        #endregion
    }
}