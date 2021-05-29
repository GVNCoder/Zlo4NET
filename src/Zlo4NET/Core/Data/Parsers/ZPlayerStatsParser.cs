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
            var statsAssignHandler = _gameSpecificStatsHandlers[statsObject.Item2];
            var stats = statsAssignHandler.Invoke(statsObject.Item1);

            return stats;
        }

        #endregion

        #region Private helpers

        private static Tuple<IDictionary<string, float>, ZGame> _ParseStatsObject(ZPacket packet)
        {
            Tuple<IDictionary<string, float>, ZGame> stats;

            using (var memoryStream = new MemoryStream(packet.Payload, false))
            using (var binaryReader = new BinaryReader(memoryStream, Encoding.ASCII))
            {
                // parse packet payload
                var targetGame = (ZGame) binaryReader.ReadByte();
                var countOfStats = binaryReader.ReadZUInt16();
                var statsDictionary = new Dictionary<string, float>(countOfStats);

                for (var i = 0; i < countOfStats; i++)
                {
                    var name = binaryReader.ReadZString();
                    var value = binaryReader.ReadZFloat();

                    statsDictionary.Add(name, value);
                }

                stats = new Tuple<IDictionary<string, float>, ZGame>(statsDictionary, targetGame);
            }

            return stats;
        }

        #endregion
    }
}